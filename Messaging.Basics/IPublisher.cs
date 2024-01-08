using Messaging.Basics;

public interface IPublisher
{
    void Publish(Message msg);
}