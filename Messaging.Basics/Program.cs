
/*
 * JOSH: Thread safe dispatcher (should it even be?)
 *       Auto registration of handled types (discuss)
 * JOHN: Break it if possible
 */

/*
 * Content based routing
 * Pending operations
 * Correlation
 */

public static class Program
{
    public static void Main()
    {
        var bus = new Bus();
        Wireup(bus);
        Send(bus);
        Console.ReadLine();
    }


    static void Wireup(Bus bus)
    {
        var cardOperations = new CardOperations(bus);
        cardOperations.SetupSubscriptions(bus);
        var bankOfSteph = new BankAccount("Alice", 1_500, bus);
        var bankOfLee = new BankAccount("Lee", 1_500, bus);
        bankOfSteph.SetupSubscriptions(bus);
        bankOfLee.SetupSubscriptions(bus);
    }


    static void Send(IPublisher publisher)
    {
        publisher.Publish(CardOperations.Charge("Alice", 10_000));
        publisher.Publish(CardOperations.Refund("Lee", 1_000));
    }
}

public class BankAccount:IHandler<BankAccount.ChargeAccount>, IHandler<BankAccount.RefundAccount>
{
    private readonly string _name;
    private decimal _amount;
    private readonly IPublisher _publisher;

    public BankAccount(string name, decimal amount, IPublisher publisher)
    {
        _name = name;
        _amount = amount;
        _publisher = publisher;
    }

    public void SetupSubscriptions(Bus bus)
    {
        bus.Subscribe<ChargeAccount>(this);
        bus.Subscribe<RefundAccount>(this);
    }

    record ChargeAccount(string Name, decimal Amount) : Message;
    record RefundAccount(string Name, decimal Amount) : Message;

    public record OperationResult(string Name, bool Successful) :Message;

    void IHandler<ChargeAccount>.Handle(ChargeAccount m)
    {
        if (_name != m.Name)
        {
            Console.WriteLine($"{_name} sings {m.Name}? {m.Name}? Who the fuck is {m.Name}?");
            _publisher.Publish(new OperationResult(_name, false));
        }
        else if (_amount - m.Amount > 0)
        {
            _amount -= m.Amount;
            Console.WriteLine($"{_name} now has {_amount:C}");
            _publisher.Publish(new OperationResult(_name, true));
        }
        else
        {
            Console.WriteLine($"{_name} cannot afford {m.Amount:C}");
            _publisher.Publish(new OperationResult(_name, false));
        }
    }

    void IHandler<RefundAccount>.Handle(RefundAccount m)
    {
        if (_name != m.Name)
        {
            Console.WriteLine($"{_name} sings {m.Name}? {m.Name}? Who the fuck is {m.Name}?");
            _publisher.Publish(new OperationResult(_name, false));
        }
        else
        {
            _amount += m.Amount;
            Console.WriteLine($"{_name} now has {_amount:C}");
            _publisher.Publish(new OperationResult(_name, true));
        }
    }

    public static Message Charge(string name, decimal amount)
    {
        VerifyParameters(name, amount);
        return new ChargeAccount(name, amount);
    }

    public static Message Refund(string name, decimal amount)
    {
        VerifyParameters(name, amount);
        return new RefundAccount(name, amount);
    }

    static void VerifyParameters(string name, decimal amount)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
    }
}

public class CardOperations : 
    IHandler<CardOperations.ChargeDebitCard>, 
    IHandler<CardOperations.RefundDebitCard>,
    IHandler<BankAccount.OperationResult>
{
    private readonly IPublisher _publisher;

    public CardOperations(IPublisher publisher)
    {
        _publisher = publisher;
    }

    void IHandler<ChargeDebitCard>.Handle(ChargeDebitCard m)
    {
        Console.WriteLine($"Charging a debit card {m.Name}, {m.Amount}");
        _publisher.Publish(BankAccount.Charge(m.Name, m.Amount));
    }

    void IHandler<RefundDebitCard>.Handle(RefundDebitCard m)
    {
        Console.WriteLine($"Refunding a debit card {m.Name}, {m.Amount}");
        _publisher.Publish(BankAccount.Refund(m.Name, m.Amount));
    }

    void IHandler<BankAccount.OperationResult>.Handle(BankAccount.OperationResult m)
    {
        if (m.Successful)
        {
            Console.WriteLine($"Something was successful for {m.Name}");
        }
        else
        {
            Console.WriteLine($"Something failed for {m.Name}");
        }
        
    }

    record ChargeDebitCard(string Name, decimal Amount):Message;
    record RefundDebitCard(string Name, decimal Amount):Message;

    public static Message Charge(string name, decimal amount)
    {
        VerifyParameters(name, amount);
        return new ChargeDebitCard(name, amount);
    }

    public static Message Refund(string name, decimal amount)
    {
        VerifyParameters(name, amount);
        return new RefundDebitCard(name, amount);
    }

    static void VerifyParameters(string name, decimal amount)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
    }

    public void SetupSubscriptions(Bus bus)
    {
        bus.Subscribe<ChargeDebitCard>(this);
        bus.Subscribe<RefundDebitCard>(this);
        bus.Subscribe<BankAccount.OperationResult>(this);
    }
}