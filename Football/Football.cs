namespace StacksTests
{
    public class FootballTests
    {
        [Fact]
        public void TestSetOfTeams()
        {
            Team chargers = new Team("Chargers");
            Assert.IsType<Team>()
        }

    }

    //  PRODUCTION CODE BELOW THIS LINE
    // *********************************

    /* you have a set of teams identified by name (Chargers, Rams, Seahawks, 49ers, Raiders, Cardinals, Bronocs)
each season every team plays one game at home and one game away
home wins are worth 2 season points
away wins are worth 3 season points
draws are always worth 1 season point
in the event of a tie the winner is determined by the difference in points scored by them vs the points scored against them
the league table should be available at any point in time with those who have played the most games ranked above those that have played less
bonus points for keeping data private*/



}