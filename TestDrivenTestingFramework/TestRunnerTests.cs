namespace TestDrivenTestingFramework;

public class TestRunnerTests
{
    [Fact]
    public void WhenRunningThreeTestsShouldOutputExpectedResults()
    {
        var testCaseDiscover = new TestCaseDiscover(new[] { typeof(TestCases) });
        var tests = testCaseDiscover.TestCases();
        var runner = new TestRunner(tests);
        var results = runner.Execute();
        
        Assert.Equal(3, results.Count);
        Assert.Equal("TestB", results[0].Name);
        Assert.Equal(TestStatus.Passed, results[0].Status);
        Assert.Equal("TestA", results[1].Name);
        Assert.Equal(TestStatus.Passed, results[1].Status);
        Assert.Equal("TestC", results[2].Name);
        Assert.Equal(TestStatus.Failed, results[2].Status);
    }
    class TestCases
    {
        [Fact]
        public void TestB()
        {
            var action = SomeMethod;
            Assert.ShouldThrow(action);
        }
        [Fact]
        public void TestA()
        {
            var action = SomeOtherMethod;
            Assert.ShouldThrow(action);
        }
        [Fact]
        public void TestC()
        {
            var action = YetAnotherMethod;
            Assert.ShouldThrow(action);
        }

        public void IAmNotATest()
        {

        }
        private static void SomeMethod()
        {
            throw new InvalidOperationException();
        }
        private static void SomeOtherMethod()
        {
            throw new InvalidOperationException();
        }

        private static void YetAnotherMethod()
        {
        
        }
    }
}