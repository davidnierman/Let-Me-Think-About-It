namespace TestDrivenTestingFramework;

class TestResult
{
    public TestResult(string scope, string name, TestStatus status, Exception? error)
    {
        Scope = scope;
        Status = status;
        Error = error;
        Name = name;
    }

    public string Scope { get; }
    public TestStatus Status { get; }
    public Exception? Error { get; }
    public string Name { get; }
}