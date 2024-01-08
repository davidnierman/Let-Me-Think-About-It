using System.Threading.Channels;

namespace Messaging.Basics;

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