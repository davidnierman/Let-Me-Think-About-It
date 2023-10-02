namespace TestDrivenTestingFramework;

class TestRunner
{
    private readonly IEnumerable<TestCase> _tests;

    public TestRunner(IEnumerable<TestCase> tests)
    {
        _tests = tests;
    }

    public IReadOnlyList<TestResult> Execute()
    {
        var results = new List<TestResult>();
        foreach (var test in _tests)
        {
            try //run some code which may or may not error
            {
                test.Run();
                results.Add(new TestResult(test.Scope, test.Name, TestStatus.Passed, null));
            }
            catch (Exception ex) 
            {
                results.Add(new TestResult(test.Scope,test.Name, TestStatus.Failed, ex.InnerException));
            }
            finally //always after we tried the code even if it errors
            {

            }
        }

        return results;
    }
}