using System.Reflection;

namespace TestDrivenTestingFramework;


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
                .Where(x=>x.GetParameters().Length == 0 && x.ReturnType == typeof(void) && x.GetCustomAttribute<FactAttribute>() != null);

            /*
             *if(theory)
             *foreach(inlinedata)
             * create testcase with parameters
             *else
             * create test case with no parameters
             */

            foreach (var method in methods)
            {
                tests.Add(new TestCase(method));
            }
        }
        return tests;
    }
}

[AttributeUsage(AttributeTargets.Method)]
class FactAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method)]
class TheoryAttribute : Attribute
{

}