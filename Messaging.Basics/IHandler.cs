namespace Messaging.Basics;

public interface IHandler<TMessage> where TMessage : Message
{
    void Handle(TMessage m);
}