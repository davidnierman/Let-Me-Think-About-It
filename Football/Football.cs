

using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace FootballTests
{
    public class FootballTests
    {

        // you have a set of teams identified by name (Chargers, Rams, Seahawks, 49ers, Raiders, Cardinals, Bronocs)
        public static IEnumerable<object[]> Teams => new object[][]
   {
        new object[] { new Team[] { new Team("Chargers"), new Team("Rams") } },
        new object[] { new Team[] { new Team("Rams"), new Team("Chargers") } },
        new object[] { new Team[] { new Team("Rams"), new Team("Chargers"), new Team("SeaHawks") } }
   };


        [Fact]
        public void TestSetOfTeams()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Assert.IsType<Team>(chargers);
            Assert.IsType<Team>(rams);
            Assert.NotEqual(chargers, rams);
        }

        [Fact]
        public void TestGame()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            Season.Game game = new(chargers, rams);
            game.Play();
            Assert.Equal(true, game.Final);
            Assert.NotNull(game.HomeTeamPoints);
            Assert.NotNull(game.AwayTeamPoints);
            Assert.Throws<Season.Game.GameAlreadyPlayed>(game.Play);
        }

        //  home wins are worth 2 season tallies
        // away wins are worth 3 season tallies
        // draws are always worth 1 season tallies
        [Fact]
        public void TestAddingSeasonPoints()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            Season.Game game = new(chargers, rams);
            game.Play();
            mmxxiii.AddSeasonPoints(game);
            Assert.Equal(game.AwayTeamPoints > game.HomeTeamPoints, mmxxiii.TallyTable[rams].Talies - 3 == mmxxiii.TallyTable[chargers].Talies); // learned probability kata
            Assert.Equal(game.AwayTeamPoints < game.HomeTeamPoints, mmxxiii.TallyTable[rams].Talies + 2 == mmxxiii.TallyTable[chargers].Talies); // learned probability kata
            Assert.Equal(game.AwayTeamPoints == game.HomeTeamPoints, mmxxiii.TallyTable[rams] == mmxxiii.TallyTable[chargers]);
            Assert.Equal(mmxxiii.TallyTable[chargers].Points, game.HomeTeamPoints - game.AwayTeamPoints);
            Assert.Equal(mmxxiii.TallyTable[rams].Points, game.AwayTeamPoints - game.HomeTeamPoints);
        }

        // each season every team plays one game at home and one game away
        [Theory]
        [MemberData(nameof(Teams))]
        public void TestCreateSeasonGames(Team[] teams)
        {
            Season mmxxiii = new Season(teams);
            var seasonGames = mmxxiii.CreateSeasonGames();
            Assert.Equal(teams.Length, seasonGames.Count);
        }

        // rank in order of most season tallies
        // in the event of a tie with season tallies the winner is determined by the difference in points scored by them vs the points scored against them
        [Fact]
        public void TestRankTeams()
        {

        }

    }

    //  PRODUCTION CODE BELOW THIS LINE
    // *********************************

    /* 
        in the event of a tie the winner is determined by the difference in points scored by them vs the points scored against them
        the league table should be available at any point in time with those who have played the most games ranked above those that have played less
        bonus points for keeping data private
    */

    public class Team
    {
        private string _name;

        public Team(string Name)
        {
            _name = Name;
        }

        public override string ToString()
        {
            return _name;
        }
    }

    class Season
    {
        private Dictionary<Team, TeamRecord> _tallyTable = new Dictionary<Team, TeamRecord>();

        public Dictionary<Team, TeamRecord> TallyTable => _tallyTable;

        public Season(Team[] teams)
        {
            foreach (var team in teams)
            {
                _tallyTable.Add(team, new TeamRecord());
            }
        }

        public HashSet<Game> CreateSeasonGames()
        {
            HashSet<Game> SeasonGames = new();
            List<Team> teamList = _tallyTable.Keys.ToList();
            Random rnd = new Random();

            HashSet<int> awayGames = new();
            for (int i = 0; i < teamList.Count;)
            {
                int randomIndex = rnd.Next(teamList.Count);
                if (awayGames.Contains(randomIndex) || randomIndex == i)
                {
                    continue;
                }
                else
                {
                    Game game = new Game(teamList[i], teamList[randomIndex]);
                    SeasonGames.Add(game);
                    awayGames.Add(randomIndex);
                    i++;
                }
            }
            return SeasonGames;
        }

        public Game PlayGame(Team homeTeam, Team awayTeam)
        {
            Game g = new Game(homeTeam, awayTeam);
            g.Play();
            return g;
        }

        // I want the below method private, but cannot test if it is...
        public void AddSeasonPoints(Game game)
        {
            var homeTeamRecord = _tallyTable[game.HomeTeam];
            var awayTeamRecord = _tallyTable[game.AwayTeam];

            homeTeamRecord.Points = game.HomeTeamPoints - game.AwayTeamPoints;
            awayTeamRecord.Points = game.AwayTeamPoints - game.HomeTeamPoints;

            if (game.HomeTeamPoints > game.AwayTeamPoints)
            {
                homeTeamRecord.Talies += 2;
            }
            else if (game.AwayTeamPoints > game.HomeTeamPoints)
            {
                awayTeamRecord.Talies += 3;
            }
            else
            {
                homeTeamRecord.Talies += 1;
                awayTeamRecord.Talies += 1;
            }

        }


        public class Game
        {
            private Team _homeTeam;
            private Team _awayTeam;
            private int _homeTeamPoints;
            private int _awayTeamPoints;
            private bool _final = false;
            private bool _homeTeamWin;

            public Team HomeTeam => _homeTeam;
            public Team AwayTeam => _awayTeam;

            public bool Final => _final;

            public bool HomeTeamWin => _homeTeamWin;
            public int HomeTeamPoints => _homeTeamPoints;
            public int AwayTeamPoints => _awayTeamPoints;

            public Game(Team homeTeam, Team awayTeam)
            {
                _homeTeam = homeTeam;
                _awayTeam = awayTeam;
            }

            public class GameAlreadyPlayed : Exception
            {
                public GameAlreadyPlayed(string message) : base(message) { }
            }

            public void Play()
            {
                if (!_final)
                {
                    Random rnd = new Random();
                    _homeTeamPoints = rnd.Next(100);
                    _awayTeamPoints = rnd.Next(100);
                    _homeTeamWin = true ? HomeTeamPoints > _awayTeamPoints : false;
                    _final = true;
                }
                else
                {
                    throw new GameAlreadyPlayed($"{_homeTeam} already played the {_awayTeam} at home");
                }

            }
        }
        public class TeamRecord
        {

            public int Talies;
            public int Points;

        }




    }

}








