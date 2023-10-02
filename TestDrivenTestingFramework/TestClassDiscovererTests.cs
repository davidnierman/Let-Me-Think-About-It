using System.Reflection;

namespace TestDrivenTestingFramework;

public class TestClassDiscovererTests
{
    [Fact]
    public void ShouldDiscoverPossibleTestClasses()
    {
        var tcd = new TestClassDiscover(Assembly.GetExecutingAssembly());

        Assert.Collection(tcd.TestClasses(),
        c => Assert.True(c.IsPublic),
            c => Assert.True(c.IsClass),
            c => Assert.False(c.IsAbstract),
            c => Assert.False(c.IsNested)
    );
}
}