using System.Collections;
using System.Reflection;

namespace TestDrivenTestingFramework;

class TestCase
{
    private readonly MethodInfo _method;

    public TestCase(MethodInfo method)
    {
        _method = method;
    }

    public string Name => _method.Name;

    public void Run()
    {
        //get an instance of the test class
        var instance = Activator.CreateInstance(_method.DeclaringType!)!;
        //call the method
        _method.Invoke(instance, new object[] { });
    }
}

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
        
        foreach(var result in results)
        {
            Console.WriteLine(result);
        }
    }

    
}

class TestClassDiscover
{
    private readonly Assembly _assembly;

    public TestClassDiscover(Assembly assembly)
    {
        _assembly = assembly;
    }

    public IEnumerable<Type> TestClasses()
    {
        var testClasses = new List<Type>();
        foreach (var testClass in _assembly.GetTypes()
                     .Where(x => x.IsClass && x.IsPublic && !x.IsNested && !x.IsAbstract))
        {
            testClasses.Add(testClass);
        }
        return testClasses;
    }
}

class TestCaseDiscover
{
    private readonly IEnumerable<Type> _testClasses;

    public TestCaseDiscover(IEnumerable<Type> testClasses)
    {
        _testClasses = testClasses;
    }

    public IEnumerable<TestCase> TestCases()
    {
        var tests = new List<TestCase>();
        foreach (var testClass in _testClasses)
        {
            var methods = testClass
                .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(x=>x.GetParameters().Length == 0 && x.ReturnType == typeof(void));
            foreach (var method in methods)
            {
                tests.Add(new TestCase(method));
            }
        }
        return tests;
    }
}
class TestRunner
{
    private readonly IEnumerable<TestCase> _tests;

    public TestRunner(IEnumerable<TestCase> tests)
    {
        _tests = tests;
    }

    public IReadOnlyList<string> Execute()
    {
        var results = new List<string>();
        foreach (var test in _tests)
        {
            try //run some code which may or may not error
            {
                test.Run();
                results.Add($"{test.Name} Passed");
            }
            catch //only on error
            {
                results.Add($"{test.Name} Failed");
            }
            finally //always after we tried the code even if it errors
            {

            }
        }

        return results;
    }
}

public class TestClassDiscovererTests
{
    public void Foo()
    {
        Assert.Equal(1, 0);
    }

    public void Bar()
    {

    }
}

public class TestRunnerTests
{
    public void WhenRunningThreeTestsShouldOutputExpectedResults()
    {
        var testCaseDiscover = new TestCaseDiscover(new[] { typeof(TestCases) });
        var tests = testCaseDiscover.TestCases();
        var runner = new TestRunner(tests);
        var results = runner.Execute();
        
        Assert.Equal(3, results.Count);
        Assert.Equal("TestB Passed", results[0]);
        Assert.Equal("TestA Passed", results[1]);
        Assert.Equal("TestC Failed", results[2]);
    }
    class TestCases
    {
        public void TestB()
        {
            var action = SomeMethod;
            Assert.ShouldThrow(action);
        }

        public void TestA()
        {
            var action = SomeOtherMethod;
            Assert.ShouldThrow(action);
        }
        public void TestC()
        {
            var action = YetAnotherMethod;
            Assert.ShouldThrow(action);
        }

        public void IAmNotATest(object becauseIHaveAParameter)
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

