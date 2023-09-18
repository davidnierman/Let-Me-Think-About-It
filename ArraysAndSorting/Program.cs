
var array = new Car[5];
array[0] = new Car("E");
array[1] = new Car("B");
array[2] = new Car("C");
array[3] = new Car("A");
array[4] = new Car("D");

var array2 = new Bar[2];
array2[0] = new Bar("Test");
array2[1] = new Bar("Code");

//Array.Sort(array);
Console.WriteLine("Sorted");
var sorted = array.OrderBy(x => x.Name).ToArray();
Print(sorted);
Console.WriteLine("Original");
Print(array);
//Print(array2);


static void Sort<T>(T[] data) where T: IComparable<T>
{
    var first = data[0];
    for (int i = 1; i < data.Length; i++)
    {
        if (first.CompareTo(data[i]) == 0)
        {
            continue;
        }
    }
}


static void Print(IFoo[] cars)
{
    for (int i = 0; i < cars.Length; i = i + 1)
    {
        Console.WriteLine(i);
        Console.WriteLine(cars[i].Name);
    }
}

class Car : IFoo, IComparable<Car>
{
    public string Name { get; }

    public Car(string name)
    {
        Name = name;
    }

    public int CompareTo(Car? other)
    {
        if(other == null) return -1;
        if (ReferenceEquals(this, other)) return 0;
        return Name.CompareTo(other.Name);
    }
}

class Bar : IFoo
{
    public string Name { get; }

    public Bar(string name)
    {
        Name = name;
    }
}

interface IFoo
{
    string Name { get; }
}