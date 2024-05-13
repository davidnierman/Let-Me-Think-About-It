using System.Text;

namespace DidYouMeanCars;

public interface IFactoryMethod<T>
{
    static abstract T Create();
}

public static class InstanceFactory
{
    public static T Create<T>() where T : IFactoryMethod<T>
    {
        return T.Create();
    }
}

public class ProductCatalog : IFactoryMethod<ProductCatalog>
{
    private readonly List<Event> _pendingEvents;
    private readonly List<Sku> _skus;

    private ProductCatalog()
    {
        _skus = new();
        _pendingEvents = new();
    }
    public ProductCatalog(string storeName):this()
    {
        Append(new ProductCatalogCreated(storeName));
    }

    public void AddSku(Sku sku)
    {
        if (_skus.Contains(sku)) return;
        
        Append(new SkuAdded(sku));
    }

    void Append(Event e)
    {
        Apply(e);
        _pendingEvents.Add(e);
    }
    public void Apply(Event @event)
    {
        switch (@event)
        {
            case ProductCatalogCreated e: Apply(e);
                break;
            case SkuAdded e: Apply(e);
                break;
            default:
                throw new NotImplementedException($"No Apply for event type {@event.GetType()}");
        }
    }

    void Apply(ProductCatalogCreated e)
    {

    }
    void Apply(SkuAdded e)
    {
        _skus.Add(e.Sku);
    }

    public IReadOnlyList<Event> ConsumePendingEvents()
    {
        var pending = _pendingEvents.ToArray();
        _pendingEvents.Clear();
        return pending;
    }

    public static ProductCatalog Create()
    {
        return new();
    }
}

public record Manufacturer(string Name);

public record Model(string Name);

public record Color(string Value);

public record Size(int Value);

public record Sku(Manufacturer Manufacturer, Model Model, Color Color, Size Size)
{
    protected virtual bool PrintMembers(StringBuilder sb)
    {
        sb.Append(Manufacturer.Name);
        sb.Append('-');
        sb.Append(Model.Name);
        sb.Append('-');
        sb.Append(Color.Value);
        sb.Append('-');
        sb.Append(Size.Value);
        return true;
    }

    public static implicit operator Sku(string value)
    {
        var parts = value.Split('-');
        if (parts.Length != 4) throw new ArgumentException("Skus have 4 parts", nameof(value));
        return new Sku(
            new Manufacturer(parts[0]),
            new Model(parts[1]),
            new Color(parts[2]),
            new Size(int.Parse(parts[3])));
    }
}

public abstract record Message;
public abstract record Event : Message;
public abstract record Command : Message;
public record ProductCatalogCreated(string Name) : Event;
public record SkuAdded(Sku Sku) : Event;