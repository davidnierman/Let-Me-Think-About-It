using System.Diagnostics;

namespace Messaging.Basics;

public class MessageTypeRouter : IHandler<Message>, IHandler<MessageTypeRouter.RegisterMessage>
{
    private readonly Dictionary<Type, List<SendMessage>> _handlers;
    public MessageTypeRouter()
    {
        _handlers = new Dictionary<Type, List<SendMessage>>();
        _handlers[typeof(RegisterMessage)] = new() {new SendMessage<RegisterMessage>(this, this)};
    }

    public void Handle(Message m)
    {
        var type = m.GetType();
        do
        {
            if (_handlers.TryGetValue(type, out var handlers))
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    handlers[i].Send(m);
                }
            }
            type = type.BaseType;
        }while(type != typeof(object) && type != null);
    }

    public static Message Subscribe<T>(IHandler<T> handler) where T : Message
    {
        return new RegisterMessage(typeof(T), new SendMessage<T>(handler, handler));
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
            return ReferenceEquals(this._handler, other._handler);
        }
    }

    class SendMessage<T> : SendMessage where T : Message
    {
        private readonly IHandler<T> _inner;

        public SendMessage(object handler, IHandler<T> inner):base(handler)
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
        if (!handlers.Contains(m.Wrapper))
        {
            handlers.Add(m.Wrapper);
        }
    }
}