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
        var sink = new Sink();
        var machine1 = new Machine(ping, sink, _logger);
        var machine2 = new Machine(pong, sink, _logger);
        Assert.True(sink.Subscribe(machine1));
        Assert.True(sink.Subscribe(machine2));
        machine1.SendMessage(sink);
    }
}


public interface ILogger
{
    public void WriteLine(string message);
}


public record Message(string M);

public class Machine
{
    private readonly Message _defaultMessage;
    private readonly Sink _sink;
    private readonly ILogger _logger;


    public Machine(Message defaultMessage, Sink sink, ILogger logger)
    {
        _defaultMessage = defaultMessage;
        _sink = sink;
        _logger = logger;
    }

    public void SendMessage(Sink sink)
    {
        sink.Broadcast(_defaultMessage, this);
    }

    public void ReceiveMessage(Message message)
    {
        _logger.WriteLine($"{_defaultMessage.M} received {message.M}");
        SendMessage(_sink);
    }
}

public class Sink
{

    private int _counter = 0;

    private readonly List<Machine> _machines;

    public Sink()
    {
        _machines = new();
    }

    public void Broadcast(Message message, Machine sender)
    {
        Console.WriteLine(_counter);
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

    public bool Subscribe(Machine machine)
    {
        _machines.Add(machine);
        return _machines.Contains(machine);
    }


}



