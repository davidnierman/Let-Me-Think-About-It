
int i = 0;
var stack = new Stack();
while (i < 10)
{
    stack.Push(i + 10);
    i++;
    Console.WriteLine($"count: {stack.Count}");
}

while (stack.Count > 0)
{
    Console.WriteLine($"contents: {stack.Pop()} count: {stack.Count}");

    //19, 18, 17 ...
}

i = 0;
while (i < 10)
{
    stack.Push(i + 10);
    i++;
    Console.WriteLine($"count: {stack.Count}");
}

i = 0;
while (i < 5)
{
    Console.WriteLine($"contents: {stack.Pop()} count: {stack.Count}");
    i++;
    //19, 18, 17 ...
}

while (stack.Count < 10)
{
    stack.Push(42);
    
    Console.WriteLine($"count: {stack.Count}");
}



while (stack.Count > 0)
{
    Console.WriteLine($"contents: {stack.Pop()} count: {stack.Count}");
}

public class Stack
{
    private int[] _data = new int[10];
    private int _index = 0;

    public void Push(int value)
    {
        _data[_index] = value;
        _index++;
    }

    public int Pop()
    {
        if (_index == 0)
        {
            throw new Exception("Stack empty");
        }
        _index--;
        var result = _data[_index];
        return result;
    }

    public int Count => _index;
}
