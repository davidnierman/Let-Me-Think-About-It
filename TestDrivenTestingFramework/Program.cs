using System.Collections;
using System.Diagnostics;

namespace TestDrivenTestingFramework;

public static class Program
{
    public static void Main()
    {

        var testClassDiscover = new TestClassDiscover(typeof(Program).Assembly);
        var testClasses = testClassDiscover.TestClasses();
        var testCaseDiscover = new TestCaseDiscover(testClasses);
        var tests = testCaseDiscover.TestCases();


        var runner = new TestRunner(tests);
        var results = runner.Execute();

        var consoleReporter = new ConsoleReporter(results);
        consoleReporter.Report();
    }

}

[AttributeUsage(AttributeTargets.Method)]
public class InlineData : Attribute
{
    public object[] Data { get; }

    public InlineData(params object[] data)
    {
        Data = data;
    }
}

public class SomeOtherTests
{
    [Theory]
    [InlineData(1,2,3)]
    public void Add(int a, int b, int expected)
    {
        Assert.Equal(expected, Add(a, b));
    }

    private static int Add(int a, int b)
    {
        return a + b;
    }
}