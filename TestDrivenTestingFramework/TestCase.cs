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
    public string Scope => _method.DeclaringType.FullName;
    public void Run()
    {
        //get an instance of the test class
        var instance = Activator.CreateInstance(_method.DeclaringType!)!;
        //call the method
        _method.Invoke(instance, new object[] { });
    }
}