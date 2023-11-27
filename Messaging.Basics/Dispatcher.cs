using System.Threading.Channels;

public class Dispatcher : IHandler<Message>, IHandler<Dispatcher.RegisterMessage>
{
    private readonly Dictionary<Type, List<SendMessage>> _handlers;
    public Dispatcher()
    {
        _handlers = new Dictionary<Type, List<SendMessage>>();
        _handlers[typeof(RegisterMessage)] = new() {new SendMessage<RegisterMessage>(this)};
    }

    public void Handle(Message m)
    {
        if (_handlers.TryGetValue(m.GetType(), out var handlers))
        {
            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].Send(m);    
            }
        }
        else
        {
            Console.WriteLine($"no handlers for {m.GetType().Name}");
        }
    }

    public static Message Register<T>(IHandler<T> handler) where T : Message
    {
        return new RegisterMessage(typeof(T), new SendMessage<T>(handler));
    }

    abstract class SendMessage
    {
        public abstract void Send(Message m);
    }

    class SendMessage<T> : SendMessage where T : Message
    {
        private readonly IHandler<T> _inner;

        public SendMessage(IHandler<T> inner)
        {
            _inner = inner;
        }
        public override void Send(Message m)
        {
            if (m is T t)
            {
                _inner.Handle(t);
            }
        }
    }

    record RegisterMessage(Type Type, SendMessage Wrapper) : Message;

    void IHandler<RegisterMessage>.Handle(RegisterMessage m)
    {
        if (!_handlers.TryGetValue(m.Type, out var handlers))
        {
            handlers = new List<SendMessage>();
            _handlers.Add(m.Type, handlers);
        }
        handlers.Add(m.Wrapper);
    }
}

public class QueuedHandler<T> : IHandler<T> where T : Message
{
    private readonly IHandler<T> _inner;
    private readonly Channel<T> _queue;

    public QueuedHandler(IHandler<T> inner)
    {
        _inner = inner;
        _queue = Channel.CreateUnbounded<T>();
        Run();
    }
    public void Handle(T msg)
    {
        while (!_queue.Writer.TryWrite(msg))
        {

        }
    }

    public async void Run()
    {
        while (true)
        {
            try
            {
                await foreach (var msg in _queue.Reader.ReadAllAsync())
                {
                    _inner.Handle(msg);
                }
            }
            catch (Exception ex)
            {
                Environment.FailFast(ex.ToString());
                return;
            }
        }
    }
}

public class Bus :IPublisher
{
    private readonly QueuedHandler<Message> _queue;

    public Bus()
    {
        _queue = new QueuedHandler<Message>(new Dispatcher());
    }
    public void Publish(Message msg)
    {
        _queue.Handle(msg);
    }

    public void Subscribe<T>(IHandler<T> handler) where T : Message
    {
        _queue.Handle(Dispatcher.Register(handler));
    }
}

public interface IPublisher
{
    void Publish(Message msg);
}