

using System.ComponentModel;

namespace FootballTests
{
    public class FootballTests
    {


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
        }

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
            // using game points to verify --> easier to test when data has accessors/getters
            Assert.Equal(game.AwayTeamPoints > game.HomeTeamPoints, mmxxiii.TallyTable[rams] - 3 == mmxxiii.TallyTable[chargers]);
            Assert.Equal(game.AwayTeamPoints < game.HomeTeamPoints, mmxxiii.TallyTable[rams] + 2 == mmxxiii.TallyTable[chargers]);
            Assert.Equal(game.AwayTeamPoints == game.HomeTeamPoints, mmxxiii.TallyTable[rams] == mmxxiii.TallyTable[chargers]);
        }

        [Fact]
        public void CannotPlayMoreThanOneGame()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            Season.Game game = new(chargers, rams);
            game.Play();
            Assert.Throws<Season.Game.GameAlreadyPlayed>(game.Play);
        }
        [Fact]
        public void PlaySeason()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            Season.Game game = new(chargers, rams);
            game.Play();
            Assert.Throws<Season.Game.GameAlreadyPlayed>(game.Play);
        }

        [Fact]
        public void TestCreateSeasonGames()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            var seasonGames = mmxxiii.CreateSeasonGames();
            Assert.Equal(teams.Length, seasonGames.Count);
        }

    }

    //  PRODUCTION CODE BELOW THIS LINE
    // *********************************

    /* you have a set of teams identified by name (Chargers, Rams, Seahawks, 49ers, Raiders, Cardinals, Bronocs)
home wins are worth 2 season points
away wins are worth 3 season points
draws are always worth 1 season point
each season every team plays one game at home and one game away
in the event of a tie the winner is determined by the difference in points scored by them vs the points scored against them
the league table should be available at any point in time with those who have played the most games ranked above those that have played less
bonus points for keeping data private*/

    class Team
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
        private Dictionary<Team, int> _tallyTable = new Dictionary<Team, int>();

        public Dictionary<Team, int> TallyTable => _tallyTable;

        public Season(Team[] teams)
        {
            foreach (var team in teams)
            {
                _tallyTable.Add(team, 0);
            }
        }

        public HashSet<Game> CreateSeasonGames()
        {
            HashSet<Game> SeasonGames = new();
            List<Team> teamList = _tallyTable.Keys.ToList<Team>();
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
                    awayGames.Add(randomIndex);
                    i++;
                }
            }
            for (int i = 0; i < teamList.Count; i++)
            {
                Game game = new Game(teamList[i], teamList[awayGames.ElementAt(i)]);
                SeasonGames.Add(game);
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
            if (game.HomeTeamPoints > game.AwayTeamPoints)
            {
                _tallyTable[game.HomeTeam] += 2;
            }
            else if (game.AwayTeamPoints > game.HomeTeamPoints)
            {
                _tallyTable[game.AwayTeam] += 3;
            }
            else
            {
                _tallyTable[game.HomeTeam] += 1;
                _tallyTable[game.AwayTeam] += 1;
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

    }


}





