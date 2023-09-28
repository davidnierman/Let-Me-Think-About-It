using Xunit.Abstractions;

namespace PingPong;

public class PingPongTests
{

    private readonly TestLogger _logger;

    public PingPongTests(ITestOutputHelper output)
    {
        _logger = new(output);
    }

    public class TestLogger : ILogger
    {
        private ITestOutputHelper _output;

        public TestLogger(ITestOutputHelper output)
        {
            _output = output;
        }
        public void WriteLine(string message)
        {
            _output.WriteLine(message);
        }

    }


    [Fact]
    public void SendPingPongs()
    {
        var ping = new Message("ping");
        var pong = new Message("pong");
        var sink = new Sink(_logger);
        var machine1 = new Machine("pinger", ping, sink, _logger);
        var machine2 = new Machine("ponger", pong, sink, _logger);
        Assert.True(sink.SubscribeToAll(machine1));
        Assert.True(sink.SubscribeToAll(machine2));
        machine1.SendMessageToEveryone(sink);
        machine2.SendMessageToEveryone(sink);
    }

    [Fact]
    public void SendAndForgetMessage()
    {
        var ping = new Message("ping");
        var emergencyPing = new UrgentMessage("ping!!!!");
        var sink = new Sink(_logger);
        var machine1 = new Machine("pinger", ping, sink, _logger);
        machine1.SendMessageToMessageSubscribers(sink, emergencyPing);
    }

    [Fact]
    public async void RouteMessage()
    {
        var ping = new Message("ping");
        var pong = new Message("pong");
        var emergencyPing = new UrgentMessage("ping!!!!");
        var sink = new Sink(_logger);
        var machine1 = new Machine("pinger", ping, sink, _logger);
        var machine2 = new Machine("ponger", pong, sink, _logger);
        sink.SubscribeToMessageType(machine2, emergencyPing);
        Assert.True(await sink.RouteAsync(emergencyPing, machine2));
        machine2.TogglePower();
        var sentResult = sink.RouteAsync(emergencyPing, machine1); // starts task here?
        await Task.Delay(2500);
        machine2.TogglePower();
        Assert.True(await sentResult);
    }


    [Fact]
    public async void RouteMessageMaxingOutRetries()
    {
        var ping = new Message("ping");
        var pong = new Message("pong");
        var emergencyPing = new UrgentMessage("ping!!!!");
        var sink = new Sink(_logger);
        var machine1 = new Machine("pinger", ping, sink, _logger);
        var machine2 = new Machine("ponger", pong, sink, _logger);
        sink.SubscribeToMessageType(machine2, emergencyPing);
        Assert.True(await sink.RouteAsync(emergencyPing, machine2));
        machine2.TogglePower();
        var sentResult = sink.RouteAsync(emergencyPing, machine1); // starts task here?
        await Task.Delay(6000);
        machine2.TogglePower();
        Assert.False(await sentResult);
    }
}

public class UrgentMessage : Message
{

    public UrgentMessage(string m) : base(m)
    {

    }
}

public interface ILogger
{
    public void WriteLine(string message);
}

public class Message
{
    private string _m;

    public string M => _m;

    public Message(string m)
    {
        _m = m;
    }
}

public class Machine
{
    private readonly string _name;
    private readonly Message _defaultMessage;
    private readonly Sink _sink;
    private readonly ILogger _logger;
    private bool _online = true;
    public string Name => _name;

    public bool Online => _online;

    public void TogglePower()
    {
        _online = !_online;
    }

    public Machine(string name, Message defaultMessage, Sink sink, ILogger logger)
    {
        _name = name;
        _defaultMessage = defaultMessage;
        _sink = sink;
        _logger = logger;
    }

    public void SendMessageToEveryone(Sink sink)
    {
        sink.Broadcast(_defaultMessage, this);
    }

    public void SendMessageToMessageSubscribers(Sink sink, Message message)
    {
        sink.RouteAsync(message, this);
    }

    public bool ReceiveMessage(Message message)
    {
        if (!_online)
        {
            throw new Exception("offline"); // "technically" not returning anything
        }

        _logger.WriteLine($"{_defaultMessage.M} received {message.M}");
        if (message is UrgentMessage)
        {
            SendMessageToMessageSubscribers(_sink, message);
            return true;
        }
        else if (message is Message)
        {
            SendMessageToEveryone(_sink);
            return true;
        }
        else
        {
            throw new InvalidOperationException("Not implemented");
        }
    }
}

public class Sink
{

    private int _counter = 0;
    private int _maxNumberOfRetries = 5;
    private readonly ILogger _logger;
    private readonly List<Machine> _machines;

    private readonly Dictionary<Type, List<Machine>> _messageSubcriptions;

    public Sink(ILogger logger)
    {
        _logger = logger;
        _machines = new();
        _messageSubcriptions = new();
    }

    public void Broadcast(Message message, Machine sender)
    {
        _counter++;
        if (_counter > 10)
        {
            return;
        }
        foreach (Machine machine in _machines)
        {
            if (machine == sender)
            {
                continue;
            }
            machine.ReceiveMessage(message);
        }
    }

    public async Task<bool> RouteAsync(Message message, Machine sender)
    {
        _counter++;
        if (_counter > _maxNumberOfRetries)
        {
            return false;
        }
        foreach (Machine machine in _messageSubcriptions[message.GetType()])
        {
            if (machine == sender) { continue; }
            try
            {
                // I think I need to pull the machine out of this equation to loosen up the coupling.. Maybe Receive Message should be async and awaited and if it times out then it is assummed offline
                machine.ReceiveMessage(message); // what to have the machine return if it is offline. It cannot return. so lack of returning? or throwing an exception. How does an offline machine throw an exception
                _counter = 0;
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"{machine.Name} is {ex.Message}");
                await Task.Delay(1000);
                return await RouteAsync(message, sender);
            }
        }
        return true;
    }

    public bool SubscribeToAll(Machine machine)
    {
        _machines.Add(machine);
        return _machines.Contains(machine);
    }
    public bool SubscribeToMessageType(Machine machine, Message message)
    {

        if (_messageSubcriptions.ContainsKey(message.GetType()))
        {
            var subsribedMachines = _messageSubcriptions[message.GetType()];
            subsribedMachines.Add(machine);
        }
        else
        {
            _messageSubcriptions.Add(message.GetType(), new List<Machine> { machine });
        }
        return _messageSubcriptions[message.GetType()].Contains(machine);
    }

}



