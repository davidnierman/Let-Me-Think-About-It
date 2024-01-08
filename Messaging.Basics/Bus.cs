namespace Messaging.Basics;

public class Bus :IPublisher
{
    private readonly QueuedHandler<Message> _queue;

    public Bus()
    {
        _queue = new QueuedHandler<Message>(new MessageTypeRouter());
    }
    public void Publish(Message msg)
    {
        _queue.Handle(msg);
    }

    public void Subscribe<T>(IHandler<T> handler) where T : Message
    {
        Publish(MessageTypeRouter.Subscribe(handler));
    }
}