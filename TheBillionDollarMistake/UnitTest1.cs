using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace TheBillionDollarMistake
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            object? foo = GetValue();
            

            int hashCode = foo!.GetHashCode();
            

            
        }

        [Fact]
        public void OutParameters()
        {
            if (TryGetValue(out var foo))
            {
                int hashCode = foo.GetHashCode();
            }
        }

        [Fact]
        public void Test2()
        {
            var foo = new Foo();
            foo.Initialize(new object(), true, new object());
            foo.UseAfterInitialize();
        }

        object? GetValue()
        {
            return new object();
        }

        bool TryGetValue([NotNullWhen(true)] out object? value)
        {
            value = new object();
            return true;
        }


        class Foo
        {
            private FooImpl? _impl;
            public Foo()
            {
                
            }

            public void Initialize(object value, bool isSomething, object? nullable)
            {
                ArgumentNullException.ThrowIfNull(nullable);
                _impl = new FooImpl(value, isSomething, nullable);
            }

            public int UseAfterInitialize()
            {
            
                if (_impl == null) throw new InvalidOperationException("Not yet initialized");
                var (value, isSomething, noLongerNullable) = _impl;
                if (isSomething)
                {
                    return value.GetHashCode();
                }

                return 0;
            }

            record FooImpl(object Value, bool IsSomething, object NoLongerNullable);
            
        }

    }
}