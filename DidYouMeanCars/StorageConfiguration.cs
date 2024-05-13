using Newtonsoft.Json.Linq;
using System.Text;

namespace DidYouMeanCars;

using AreaDefinition = (int Aisles, int ShelfSetsPerAisle, int ShelvesPerShelfSet, int BinsPerShelf);

public record AreaDefined(
    string Area,
    int Aisles,
    int ShelfSetsPerAisle,
    int ShelvesPerShelfSet,
    int BinsPerShelf
) : Event;

public class StorageConfiguration
{
    private readonly List<Event> _pendingEvents;
    private readonly Dictionary<string,AreaDefinition> _areas;
    private readonly Dictionary<string, int> _transitLocations;
    public StorageConfiguration()
    {
        _pendingEvents = new List<Event>();
        _areas = new (StringComparer.OrdinalIgnoreCase);
        _transitLocations = new(StringComparer.OrdinalIgnoreCase);
    }

    public void DefineArea(string area, int aisles, int shelfSetsPerAisle, int shelvesPerShelfSet, int binsPerShelf)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(area);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(aisles);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(shelfSetsPerAisle);
        if (_areas.TryGetValue(area, out var definition))
        {
            if (definition == (aisles, shelfSetsPerAisle, shelvesPerShelfSet, binsPerShelf)) return;
            throw new InvalidOperationException("Area already defined with a different definition");
        }

        Append(new AreaDefined(area, aisles, shelfSetsPerAisle, shelvesPerShelfSet,
            binsPerShelf));
    }

    void Apply(AreaDefined @event)
    {
        _areas.Add(@event.Area,
            (
                @event.Aisles, 
                @event.ShelfSetsPerAisle, 
                @event.ShelvesPerShelfSet,
                @event.BinsPerShelf));
    }
    
    void Append(Event @event)
    {
        Apply(@event);
        _pendingEvents.Add(@event);
    }

    public IReadOnlyList<Event> ConsumePendingEvents()
    {
        var pending = _pendingEvents.ToArray();
        _pendingEvents.Clear();
        return pending;
    }

    public void Apply(Event @event)
    {
        switch (@event)
        {
            case AreaDefined e: Apply(e);
                break;
            case TransitLocationDefined e: Apply(e);
                break;
        }
    }

    public void DefineTransitLocation(string name, int capacity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);
        if (_transitLocations.TryGetValue(name, out var definition))
        {
            if(definition == capacity) return;
            throw new InvalidOperationException("Transit location with same name already added");
        }
        Append(new TransitLocationDefined(name, capacity));
    }

    void Apply(TransitLocationDefined e)
    {
        _transitLocations.Add(e.Name, e.Capacity);
    }
}

public record TransitLocationDefined(string Name, int Capacity) : Event;

public record struct TransitLocation(string Name, int Capacity)
{
    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append(Name).Append("-").Append(Capacity);
        return true;
    }

    public static implicit operator TransitLocation(string value)
    {
        var parts = value.Split('-');
        if (parts.Length != 2) throw new ArgumentException("TransitLocation has 2 parts", nameof(value));
        return new TransitLocation(parts[0], int.Parse(parts[1]));
    }
}
public record struct StorageLocation(Area Area, Aisle Aisle, Bay Bay, Shelf Shelf, Bin Bin)
{
    public override string ToString()
    {
        return $"{Area.Name}-{Aisle.Number}-{Bay.Name}-{Shelf.Number}-{Bin.Name}";
    }

    public static implicit operator StorageLocation(string value)
    {
        var parts = value.Split('-');
        if (parts.Length != 5) throw new ArgumentException("Location has 5 parts", nameof(value));
        return new StorageLocation(
            new Area(parts[0]),
            new Aisle(int.Parse(parts[1])),
            new Bay(parts[2]),
            new Shelf(int.Parse(parts[3])),
            new Bin(parts[4]));
    }
}

public record struct Area(string Name)
{
    public static explicit operator Area(string name) => new(name);
}

public record struct Aisle
{
    public uint Number { get; }

    public Aisle(int number)
    {
        if (number < 0) throw new ArgumentOutOfRangeException(nameof(number));
        Number = (uint)number;
    }
}

public record struct Bay(string Name)
{
    public static explicit operator Bay(string name) => new(name);
}

public record struct Shelf
{
    public uint Number { get; }

    public Shelf(int number)
    {
        if (number < 0) throw new ArgumentOutOfRangeException(nameof(number));
        Number = (uint)number;
    }
} // horizontal component

public record struct Bin(string Name)
{
    public static explicit operator Bin(string name) => new(name);
}