namespace Messaging.Basics;

public class ContentBasedRouter : IHandler<Message>, IHandler<ContentBasedRouter.RegisterMessage>
{
    private readonly Dictionary<Type, List<SendMessage>> _handlers;
    public ContentBasedRouter()
    {
        _handlers = new Dictionary<Type, List<SendMessage>>();
        _handlers[typeof(RegisterMessage)] = new() {new SendMessage<RegisterMessage, RegisterMessage>(this, this, _ => true)};
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
            Console.WriteLine($"It would sure be nice to have a better message here {m.GetType().Name}");
        }
    }

    public static Message Register<TBase, TMessage>(IHandler<TMessage> handler, Func<TBase, bool> filter) where TMessage : Message, TBase
    {
        return new RegisterMessage(typeof(TMessage), new SendMessage<TBase, TMessage>(handler, handler, filter));
    }

    abstract class SendMessage : IEquatable<SendMessage>
    {
        private readonly object _handler;

        public SendMessage(object handler)
        {
            _handler = handler;
        }
        public abstract void Send(Message m);

        public bool Equals(SendMessage? other)
        {
            if(ReferenceEquals(null, other)) return false;
            return ReferenceEquals(_handler, other._handler);
        }
    }

    class SendMessage<TBase, TMessage> : SendMessage where TMessage : Message, TBase
    {
        private readonly IHandler<TMessage> _inner;
        private readonly Func<TBase, bool> _filter;

        public SendMessage(object handler, IHandler<TMessage> inner, Func<TBase, bool> filter):base(handler)
        {
            _inner = inner;
            _filter = filter;
        }
        public override void Send(Message m)
        {
            if (m is TMessage t && _filter(t))
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
        if (!handlers.Contains(m.Wrapper))
        {
            handlers.Add(m.Wrapper);
        }
    }

    public static void EnableFor(Bus bus)
    {
        bus.Subscribe<RegisterMessage>(new ContentBasedRouter());
    }
}