
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Pattern.Tests
{
    public class SingletonTests
    {
        [Fact]
        public void StaticSingletonExample() //issues?
        {
            Assert.Equal(1, StaticSingleton.Value);
        }


        [Fact]
        public void NonStaticSingleton()
        {
            var instance = Singleton.Instance;
            Assert.Equal(1, instance.Value);
        }

        [Fact]
        public void NonStaticSingletonOneInstance()
        {
            var instance = Singleton.Instance;
            Assert.Same(instance, Singleton.Instance);
        }

        [Fact]
        public void CodedSingleton()
        {
            var foo = new Foo();
            Assert.Same(foo.Instance, foo.Instance);
        }

        [Fact]
        public void SingletonViaDI()
        {
            var sc = new ServiceCollection();
            sc.AddScoped<Bar>();
            var sp = sc.BuildServiceProvider();
            using var iReallyDontCare = sp.CreateScope();
            using var iReallyDontCare2 = sp.CreateScope();
            Assert.NotSame(iReallyDontCare.ServiceProvider.GetService<Bar>(), iReallyDontCare2.ServiceProvider.GetService<Bar>());
        }

        [Fact]
        public void NonStaticSingletonValueSharing()
        {
            var instance = Singleton.Instance;
            Enumerable.Range(1, 5).Aggregate(0, (a, b) => a + b);
            instance.Value = 2;
            var valueBeforeChange = Singleton.Instance.Value;
            Assert.Equal(instance.Value, valueBeforeChange);
        }
        //Singleton

        public class Singleton // Highlander pattern: There can only be one
        {
            public static Singleton Instance = new Singleton() { Value = 1 };
            public int Value { get; set; }
        }

        public class Bar{}

        public class Foo
        {
            public Foo()
            {
                Instance = new Singleton();
            }

            public Singleton Instance { get; set; }

            public class Singleton
            {
                public int Value { get; set; }
            }
        }

        public static class StaticSingleton
        {
            public static readonly int Value = 1;
            
        }
    }

    public class DoubleDispatchTests
    {
        [Fact]
        public void CheckArea()
        {
            //Assert.Equal(4, new Shape.Square(2).Area());
            //Assert.Equal(2, new Shape.Triangle(2, 2).Area());

            //Assert.Equal(6, new Shape.Composite(new Shape.Square(2), new Shape.Triangle(2,2)).Area());
            //is the shape a square and a triangle

            var shape = new Shape.Square(2);
            var calculator = new CalculateArea();
            Assert.Equal(4m,calculator.Visit(shape));
            Assert.Equal(6m,calculator.Area(new Shape.Composite(new Shape.Square(2), new Shape.Triangle(2,2))));
            var counter = new ShapeCounter();
            Assert.Equal(2,counter.Count(new Shape.Composite(new Shape.Square(2), new Shape.Triangle(2,2))));

            Assert.Equal((decimal)Math.PI, calculator.Area(new Shape.Circle(1)));
            Assert.Equal(1, counter.Count(new Shape.Circle(1)));
        }
    }
    public interface IVisitor<T>
    {
        T Visit(Shape.Square shape);
        T Visit(Shape.Composite shape);
        T Visit(Shape.Triangle shape);
        T Visit(Shape.Circle shape);
        T Visit(Rectangle shape);
    }

    public class Rectangle : Shape
    {
        public int X { get; }
        public int Y { get; }

        public Rectangle(int x, int y)
        {
            X = x;
            Y = y;
        }
        protected override T Visit<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
    public class ShapePrinter : Shape.VisitorAcceptor<string>, IVisitor<string>
    {
        public string Visit(Shape.Square shape)
        {
            return $"Square({shape.Side})";
        }

        public string Visit(Shape.Composite shape)
        {
            return Accept(shape.A, this) + Environment.NewLine+ Accept(shape.B, this);
        }

        public string Visit(Shape.Triangle shape)
        {
            return $"Triangle({shape.A}/{shape.B})";
        }

        public string Visit(Shape.Circle shape)
        {
            return $"Circle({shape.Radius})";
        }

        public string Visit(Rectangle shape)
        {
            return $"Rectangle X:{shape.X} Y:{shape.Y}";
        }
    }
    public class ShapeCounter : Shape.VisitorAcceptor<int>, IVisitor<int>
    {
        public int Count(Shape shape)
        {
            return Accept(shape, this);
        }
        public int Visit(Shape.Square shape)
        {
            return 1;
        }

        public int Visit(Shape.Composite shape)
        {
            return Accept(shape.A, this) + Accept(shape.B, this);
        }

        public int Visit(Shape.Triangle shape)
        {
            return 1;
        }

        public int Visit(Shape.Circle shape)
        {
            return 1;
        }

        public int Visit(Rectangle shape) => 1;
    }
    public class CalculateArea : Shape.VisitorAcceptor<decimal>, IVisitor<decimal>
    {
        public decimal Area(Shape shape)
        {
            return Accept(shape, this);
        }

        public decimal Visit(Shape.Square shape)
        {
            return shape.Side * shape.Side;
        }

        public decimal Visit(Shape.Composite shape)
        {
            return Accept(shape.A, this) + Accept(shape.B, this) ;
        }

        public decimal Visit(Shape.Triangle shape)
        {
            return 0.5m * shape.A * shape.B;
        }

        public decimal Visit(Shape.Circle shape)
        {
            return (decimal)Math.PI * shape.Radius * shape.Radius;
        }

        public decimal Visit(Rectangle r) => r.X * r.Y;
    }
    public abstract class Shape
    {
        public abstract class VisitorAcceptor<T>
        {
            public T Accept(Shape shape, IVisitor<T> visitor)
            {
                return shape.Visit(visitor);
            }
        }
        
        protected abstract T Visit<T>(IVisitor<T> visitor);

        public class Composite : Shape
        {
            public Shape A { get; }
            public Shape B { get; }

            public Composite(Shape a, Shape b)
            {
                A = a;
                B = b;
            }

            protected override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Square : Shape
        {
            public readonly int Side;

            public Square(int side)
            {
                Side = side;
            }

            protected override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Triangle: Shape
        {
            public int A { get; }
            public int B { get; }

            public Triangle(int a, int b)
            {
                A = a;
                B = b;
            }

            protected override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Circle : Shape
        {
            public int Radius { get; }

            public Circle(int radius)
            {
                Radius = radius;
            }

            protected override T Visit<T>(IVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}