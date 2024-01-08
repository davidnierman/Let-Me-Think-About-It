namespace Messaging.Basics.Tests
{
    public class MessageTypeRouterTests : IHandler<MessageTypeRouterTests.IsThereAnybodyOutThere>
    {
        private readonly MessageTypeRouter _sut;
        private readonly TaskCompletionSource<Message> _received;
        private readonly CancellationToken _ct;

        public MessageTypeRouterTests()
        {
            _sut = new MessageTypeRouter();
            _received = new TaskCompletionSource<Message>(TaskCreationOptions.RunContinuationsAsynchronously);
            _ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(100)).Token;
        }

        [Fact]
        public void ShoutingIntoTheVoid()
        {
            var ex = Record.Exception(() =>_sut.Handle(new NobodyIsListening()));
            Assert.Null(ex);
        }
        [Fact]
        public async Task AnotherBrickInTheWall()
        {
            _sut.Handle(MessageTypeRouter.Subscribe(this));
            var m = new IsThereAnybodyOutThere();
            var ex = Record.Exception(() => _sut.Handle(m));
            Assert.Null(ex);
            var received = await _received.Task.WaitAsync(_ct);
            Assert.Equal(m, received);
        }

        void IHandler<IsThereAnybodyOutThere>.Handle(IsThereAnybodyOutThere m)
        {
            _received.SetResult(m);
        }

        record IsThereAnybodyOutThere : Message;
        record NobodyIsListening : Message;
    }

    public class HowDoWeDealWithTimeouts : IHandler<HowDoWeDealWithTimeouts.Response>, IHandler<HowDoWeDealWithTimeouts.Timeout>
    {
        private readonly MessageTypeRouter _sut;
        private readonly TaskCompletionSource _timedOut;
        private readonly TaskCompletionSource _received;
        private readonly CancellationToken _ct;
        private readonly DelayedSendService _timerService;

        public HowDoWeDealWithTimeouts()
        {
            _sut = new MessageTypeRouter();
            _timedOut = new TaskCompletionSource();
            _received = new TaskCompletionSource();
            _ct = new CancellationTokenSource(TimeSpan.FromMilliseconds(100)).Token;
            _timerService = new DelayedSendService(_sut);
            _timerService.Initialize(_sut);
            _sut.Handle(MessageTypeRouter.Subscribe<Timeout>(this));
            _sut.Handle(MessageTypeRouter.Subscribe<Response>(this));
        }

        [Fact]
        public async Task CanTimeout()
        {
            Assert.False(_timedOut.Task.IsCompleted);
            _sut.Handle(DelayedSendService.Later(TimeSpan.FromDays(1), new Timeout()));
            _timerService.UpdateTime(TimeSpan.FromDays(2));
            await _timedOut.Task.WaitAsync(_ct);
        }
        [Fact]
        public async Task CanGetResponse()
        {
            Assert.False(_received.Task.IsCompleted);
            _sut.Handle(new Response());
            await _received.Task.WaitAsync(_ct);
        }
        void IHandler<Response>.Handle(Response m)
        {
            _received.TrySetResult();
        }

        void IHandler<Timeout>.Handle(Timeout m)
        {
            _timedOut.TrySetResult();
        }

        record Response : Message;
        record Timeout : Message;
    }
    
    class DelayedSendService : IHandler<DelayedSendService.SendAfter>
    {
        private readonly IHandler<Message> _output;
        private static TimeSpan _now;
        private readonly List<SendAfter> _pending;

        public DelayedSendService(IHandler<Message> output)
        {
            _output = output;
            _now = TimeSpan.Zero;
            _pending = new();
        }
        public static Message Later(TimeSpan time, Message m)
        {
            return new SendAfter(_now.Add(time), m);
        }

        public void UpdateTime(TimeSpan now)
        {
            _now = now;
            for (int i = 0; i < _pending.Count; i++)
            {
                if(_pending[i].SendAt < now)
                    _output.Handle(_pending[i].ToSend);
            }
        }

        void IHandler<SendAfter>.Handle(SendAfter m)
        {
            if (_now >= m.SendAt)
            {
                _output.Handle(m.ToSend);
            }
            else
            {
                _pending.Add(m);
            }
        }

        record SendAfter(TimeSpan SendAt, Message ToSend) : Message;

        public void Initialize(IHandler<Message> bus)
        {
            bus.Handle(MessageTypeRouter.Subscribe(this));
        }
    }
}