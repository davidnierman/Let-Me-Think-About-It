using System.Collections;
using System.Collections.Generic;

namespace StacksTests
{
    public class EveryLoopIsAWhileLoop
    {

        [Fact]
        public void ForLoop()
        {
            int count = 0;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(i);
                count += i;
            }
            Assert.Equal(10, count);
        }
        [Fact]
        public void ForLoopAsWhileLoop()
        {
            int count = 0;
            int i = 0;
            while (i < 5)
            {
                Console.WriteLine(i);
                count += i;
                i++;
            }
            Assert.Equal(10, count);
        }

        [Fact]
        public void DoWhileLoop()
        {
            // repeats until it does not meet the condition
            int i = 0;
            do
            {
                Console.WriteLine(i);
                Assert.True(i < 10);
                i++;
            }
            while (i < 10);
            Assert.Equal(10, i);

            // done once even when it does not meet the condition
            int j = 0;
            do
            {
                Console.WriteLine(j);
                j = 100;
            }
            while (j < 10);
            Assert.Equal(100, j);
        }

        [Fact]
        public void DoWhileLoopAsWhileLoop()
        {
            // repeats until it does not meet the condition
            bool hasDoneOneOnce = false;
            int i = 0;
            while (i < 10 || !hasDoneOneOnce)
            {
                hasDoneOneOnce = true;
                Console.WriteLine(i);
                Assert.True(i < 10);
                i++;
            }
            Assert.Equal(10, i);

            // done once even when it does not meet the condition
            int j = 0;
            hasDoneOneOnce = false;
            while (i < 10 || !hasDoneOneOnce)
            {
                hasDoneOneOnce = true;
                Console.WriteLine(j);
                j = 100;
            }
            Assert.Equal(100, j);


        }


        [Fact]
        public void ForEachInAListLoop()
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            int currentNumber = 0;

            foreach (int num in numbers)
            {
                Console.WriteLine(num);
                currentNumber = num;
            }
            Assert.Equal(5, currentNumber);
        }

        [Fact]
        public void ForEachInAListLoopAsWhileLoop()
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            int currentNumber = 0;
            int index = 0;
            while (index < numbers.Count)
            {
                int num = numbers[index];
                Console.WriteLine(num);
                currentNumber = num;
                index++;
            }
            Assert.Equal(5, currentNumber);
        }
        [Fact]
        public void ForEachInADictionaryLoop()
        {
            string currentName = "Dave";
            int currentAge = 0;
            Dictionary<string, int> ages = new Dictionary<string, int>
            {
            { "Alice", 30 },
            { "Bob", 25 },
            { "Charlie", 28 }
            };

            foreach (var pair in ages)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value} years old");
                currentName = pair.Key;
                currentAge = pair.Value;
            }
            Assert.Equal("Charlie", currentName);
            Assert.Equal(28, currentAge);
        }

        [Fact]
        public void ForEachInADictionaryLoopAsWhileLoop()
        {
            string currentName = "Dave";
            int currentAge = 0;
            Dictionary<string, int> ages = new Dictionary<string, int>
        {
            { "Alice", 30 },
            { "Bob", 25 },
            { "Charlie", 28 }
        };

            var enumerator = ages.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                Console.WriteLine($"{pair.Key}: {pair.Value} years old");
                currentName = pair.Key;
                currentAge = pair.Value;
            }
            Assert.Equal("Charlie", currentName);
            Assert.Equal(28, currentAge);
        }
        [Fact]
        public void CustomEnumerator()
        {
            Bird bird = new(1, 2);
            int i = 1;

            while (bird.MoveNext())
            {
                Assert.Equal(i, bird.Current);
                i++;
            }
        }
        [Fact]
        public void CustomEnumeratoForEachr()
        {
            Bird bird = new(1, 2);
            int i = 1;

            foreach (var d in bird)
            {
                Assert.Equal(i, d);
                i++;
            }
        }
        [Fact]
        public void CustomEnumeratoForEachDuck()
        {
            Duck duck = new(1, 2);
            int i = 1;

            foreach (var d in duck)
            {
                Assert.Equal(i, d);
                i++;
            }
        }
    }
}



class Duck
{
    public int A;
    public int B;

    private int _counter;

    public Duck(int a, int b)
    {
        A = a;
        B = b;
        _counter = -1;
    }

    public object Current => GetType().GetFields()[_counter].GetValue(this);

    public bool MoveNext()
    {
        _counter += 1;
        return _counter < 2;
    }

    public Duck GetEnumerator()
    {
        return this;
    }
}

class Bird : IEnumerable, IEnumerator
{
    public int A;
    public int B;

    private int _counter;

    public Bird(int a, int b)
    {
        A = a;
        B = b;
        _counter = -1;
    }


    public object Current => GetType().GetFields()[_counter].GetValue(this);

    public bool MoveNext()
    {
        _counter += 1;
        return _counter < 2;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator() => this;
}