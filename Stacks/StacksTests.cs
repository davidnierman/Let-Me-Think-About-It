namespace StacksTests
{
    public class StacksTests
    {
        [Fact]
        public void TestStack()
        {
            int i = 0;
            var stack = new Stack(10);

            // push numbers on the stack
            while (i < 10)
            {
                stack.Push(i + 100);
                Console.WriteLine($"pushing {i + 100} onto the stack in slot {stack.Count - 1}");
                i++;
            }

            // pop numbers off the stack
            while (stack.Count > 0)
            {
                Console.WriteLine($"popping {stack.Pop()} off the stack");
            }
        }

    }

    class Stack
    {
        private int[] _data;
        private int _index = 0;

        public Stack(int size)
        {
            _data = new int[size];
        }

        public void Push(int value)
        {
            _data[_index] = value;
            _index++;
        }

        public int Pop()
        {
            _index--;
            var result = _data[_index];

            return result;
        }

        public int Count => _index;
    }
}