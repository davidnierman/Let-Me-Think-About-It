using Xunit.Abstractions;

namespace PingPong
{
    public class MailingTests
    {
        private readonly TestLogger _logger;

        public MailingTests(ITestOutputHelper output)
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
        public void TestLoggerWriteline()
        {
            _logger.WriteLine("hi!");
        }

        [Fact]
        public void TestActorState()
        {
            Actor component = new PubSub<AMessage>(_logger);
            Assert.NotNull(component);
            Assert.Equal(0, ((short)component.State));
            component.UpdateState(State.Active);
            Assert.Equal(1, ((short)component.State));
            component.UpdateState(State.Active);
            Assert.Equal(1, ((short)component.State));
            component.UpdateState(State.Inactive);
            Assert.Equal(0, ((short)component.State));
            component.UpdateState(State.Inactive);
            Assert.Equal(0, ((short)component.State));
        }

        [Theory]
        [InlineData(1000000)]
        public void TestActorGuid(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                Actor componentA = new PubSub<AMessage>(_logger);
                Actor componentB = new PubSub<AMessage>(_logger);
                Assert.NotEqual(componentA.Id, componentB.Id);
                Assert.NotEqual(componentA.GetHashCode(), componentB.GetHashCode()); // could use hash instead?
            }
        }
        [Theory]
        [InlineData(1000000)]
        public void TestMessageGuid(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                PubSub<AMessage> componentA = new(_logger);
                PubSub<AMessage> componentB = new(_logger);
                MessageWithReturnActor messageA = new(componentA, "hi!");
                MessageWithReturnActor messageB = new(componentB, "Hello!");
                Assert.NotEqual(messageA.Id, messageB.Id);
                Assert.NotEqual(messageA.Message, messageB.Message);
                Assert.NotEqual(messageA.GetHashCode(), messageB.GetHashCode()); // could use hash instead?
            }
        }



        [Fact]
        public void TestMailBox()
        {
            PubSub<AMessage> componentA = new(_logger);
            MessageReceiver componentB = new(_logger);
            MessageMailbox mailboxB = new(componentB, _logger);
            MessageWithReturnActor message = new(componentA, "hi!"); // I should add create message to the pubsub class
            componentA.Subscribe(mailboxB);
            componentA.Publish(message);
            mailboxB.Handle();
        }


        [Fact]
        public void TestResponse()
        {
            PubSub<AMessage> componentA = new(_logger);
            PubSub<AMessage> componentB = new(_logger);
            MessageMailbox mailboxA = new(componentA, _logger);
            MessageMailbox mailboxB = new(componentB, _logger);
            MessageWithReturnActor message = new(componentA, "hi!"); // I should add create message to the pubsub class
            componentA.Subscribe(mailboxB);
            componentB.Subscribe(mailboxA);
            componentA.Publish(message);
            mailboxB.Handle();
            mailboxB.Handle();
            mailboxA.Handle();
            //mailboxB.Handle();
            //mailboxA.Handle();
        }


    }

    [Flags]
    public enum State : short
    {
        Inactive,
        Active
    }


    public abstract class Actor
    {

        public ILogger Logger => _logger;
        private readonly Guid _id = Guid.NewGuid();
        public Guid Id => _id;

        private State _state = State.Inactive;
        private ILogger _logger;

        public State State => _state;

        public void UpdateState(State state)
        {
            _state = state;
        }

        public Actor(ILogger logger)
        {
            _logger = logger;
        }

    }


    public interface IHandler
    {
        public void Handle();
    }

    public interface IMessageHandler<T> where T : AMessage
    {
        void Handle(T message);
    }

    public abstract class AMessage
    {

        private readonly Guid _id = Guid.NewGuid();
        private string _message;


        public Guid Id => _id;
        public string Message => _message;

        public AMessage(string message)
        {
            _message = message;

        }
    }

    public class MessageWithReturnActor : AMessage
    {
        private readonly IMessageReceiver<AMessage> _sender;    // for this model we are going to assume that to send a message, you need to be able receive one
        public MessageWithReturnActor(IMessageReceiver<AMessage> sender, string message) : base(message)
        {
            _sender = sender;
        }
    }


    public interface IQueue<T>
    {
        public T Dequeue();

        public void Enqueue(T message);
    }

    public abstract class Mailbox<T> : MessageReceiver, IHandler, IQueue<T>, IMessageReceiver<T> where T : AMessage
    {
        private ILogger _logger;
        private Actor _owner;
        private Queue<T> _queue = new();
        public int Count => _queue.Count;
        public Mailbox(MessageReceiver owner, ILogger logger) : base(logger)
        {
            _logger = logger;
            _owner = owner;
        }

        public T Dequeue()
        {
            return _queue.Dequeue();

        }

        public void Enqueue(T message)
        {
            _queue.Enqueue(message);
        }

        public void Handle()
        {
            _logger.WriteLine($"{_owner.Id} is going through mailbox: {this.Id}");

            if (_queue.Count > 0)
            {
                if (_owner is PubSub<AMessage>)
                {
                    var o = _owner as PubSub<AMessage>;
                    o.ReceiveMessage(Dequeue());
                }
                else if (_owner is MessageReceiver)
                {

                    var o = _owner as MessageReceiver;
                    o.ReceiveMessage(Dequeue());
                }
            }
            else
            {
                _logger.WriteLine($"mailbox: {this.Id} is Empty!");
            }



        }

        public void ReceiveMessage(T message)
        {
            _logger.WriteLine($"mailbox:{this.Id} received the message {message.Id}");
            Enqueue(message);
        }
    }

    public class MessageMailbox : Mailbox<AMessage>
    {
        public MessageMailbox(MessageReceiver owner, ILogger logger) : base(owner, logger)
        {
        }
    }

    public interface IPublisher<T> where T : AMessage
    {
        public void Publish(T message);

        public void Subscribe(Actor subscribe);
        public void Unsubscribe(T actor);

    }

    public interface IMessageReceiver<T> where T : AMessage // just because a component can receive a message does that mean they handle it??
    {
        public void ReceiveMessage(T message);
    }

    public class MessageReceiver : Actor, IMessageReceiver<AMessage>
    {
        private readonly ILogger _logger;
        public MessageReceiver(ILogger logger) : base(logger)
        {
            _logger = logger;
        }
        public void ReceiveMessage(AMessage message)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageReader : IMessageHandler<AMessage>, IMessageReceiver<AMessage>
    {

        private ILogger _logger;
        public MessageReader(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(AMessage message)
        {
            _logger.WriteLine(message.Message);
        }

        public void ReceiveMessage(AMessage message)
        {
            Handle(message);
        }
    }

    public class MessagePublisher : Actor, IPublisher<AMessage>
    {
        private List<IMessageReceiver<AMessage>> _subscribers = new();

        public MessagePublisher(ILogger logger) : base(logger)
        {
        }

        public void Publish(AMessage message)
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.ReceiveMessage(message);
            }
        }

        public void Subscribe(Actor subscriber)
        {
            _subscribers.Add((IMessageReceiver<AMessage>)subscriber);
        }

        public void Unsubscribe(Actor actor)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(AMessage actor)
        {
            throw new NotImplementedException();
        }
    }

    public class PubSub<T> : MessageReceiver, IMessageHandler<T>, IPublisher<T>, IMessageReceiver<T> where T : AMessage
    {
        private readonly ILogger _logger;
        private List<Actor> _subscribers = new();
        public PubSub(ILogger logger) : base(logger)
        {
            _logger = logger;
        }
        public void Handle(T message)
        {
            Publish(message);
        }

        public void Publish(T message)
        {
            foreach (var subscriber in _subscribers)
            {
                _logger.WriteLine($"PubSub Component: {this.Id} is sending a message to mailbox {subscriber.Id}");

                if (subscriber is IMessageReceiver<T>)
                {
                    var messageReceiver = subscriber as IMessageReceiver<T>;
                    messageReceiver.ReceiveMessage(message);
                }
            }
        }

        public void ReceiveMessage(T message)
        {
            _logger.WriteLine($"Component {this.Id} received the message {message.Message}");
            Handle(message);
        }

        public void Subscribe(Actor subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(T actor)
        {
            throw new NotImplementedException();
        }

    }

}



