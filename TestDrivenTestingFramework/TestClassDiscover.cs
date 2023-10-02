using System.Reflection;

namespace TestDrivenTestingFramework;

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