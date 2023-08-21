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
        public void TestHomeAndAwaySeasonPoints()
        {
            Team chargers = new Team("Chargers");
            Team rams = new Team("Rams");
            Team[] teams = { chargers, rams };
            Season mmxxiii = new Season(teams);
            mmxxiii.AddSeasonPoints(chargers, 10, rams, 21);
            Assert.Equal(0, mmxxiii.TallyTable[chargers]);
            Assert.Equal(3, mmxxiii.TallyTable[rams]);
            mmxxiii.AddSeasonPoints(chargers, 35, rams, 21);
            Assert.Equal(2, mmxxiii.TallyTable[chargers]);
            Assert.Equal(3, mmxxiii.TallyTable[rams]);
            mmxxiii.AddSeasonPoints(chargers, 14, rams, 14);
            Assert.Equal(3, mmxxiii.TallyTable[chargers]);
            Assert.Equal(4, mmxxiii.TallyTable[rams]);
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

        public Game PlayGame(Team homeTeam, Team awayTeam)
        {
            Game g = new Game(homeTeam, awayTeam);
            return g;
        }

        // I want the below method private, but cannot test if it is...
        public void AddSeasonPoints(Team homeTeam, int homeTeamGamePoints, Team awayTeam, int awayTeamGamePoints)
        {
            if (homeTeamGamePoints > awayTeamGamePoints)
            {
                _tallyTable[homeTeam] += 2;
            }
            else if (awayTeamGamePoints > homeTeamGamePoints)
            {
                _tallyTable[awayTeam] += 3;
            }
            else
            {
                _tallyTable[homeTeam] += 1;
                _tallyTable[awayTeam] += 1;
            }
        }


        public class Game
        {
            private Team _homeTeam;
            private Team _awayTeam;
            private int _homeTeamPoints;
            private int _awayTeamPoints;

            public int HomeTeamPoints => _homeTeamPoints;
            public int AwayTeamPoints => _awayTeamPoints;

            public Game(Team homeTeam, Team awayTeam)
            {
                _homeTeam = homeTeam;
                _awayTeam = awayTeam;
            }

            public void Play()
            {
                Random rnd = new Random();
                int _homeTeamPoints = rnd.Next(100);
                int _awayTeamPoints = rnd.Next(100);

            }


        }

    }


}





