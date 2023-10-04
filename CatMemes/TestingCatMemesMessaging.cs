// my job is to send cat memes --> Idk who is interested in cat memes, I just create the cat memes and send them;

// THE TEST ABOVE AND BELOW ARE INDEPENDENTLY TESTABLE

// people interested in cat memes sign up/subsribe to cat memes;
// people who signed up for cat memes should receive anonymous cat memes --> They don't know who sent them;

//BONUS ALL THE ABOVE BUT YOU CAN ALSO SEND DOG MEMES --> PEOPLE WHO SUBSCRIBE TO CAT RECEIVE CAT, PEOPLE WHO SUBSCRIBE TO DOG RECEIVE DOG AND PEOPLE WHO SUBSCRIBE TO BOTH RECEIVE DOG & CAT

namespace CatMemes
{
    public class TestingCatMemeMessaging
    {
        [Fact]
        public void CanSendCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemeSender = new CatMemeSender();
            catMemeSender.Handle(catMeme);
            Assert.NotNull(() => catMemeSender.Handle(catMeme));
        }

        [Fact]
        public void CanSubscribeToCatMemes()
        {
            var catMemeReceiver = new CatMemeReceiver();
            var catMemePublisher = new CatMemePublisher();
            Assert.NotNull(() => catMemePublisher.Subscribe(catMemeReceiver));
        }

        [Fact]
        public void CanPublishCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemePublisher = new CatMemePublisher();
            Assert.NotNull(() => catMemePublisher.Publish(catMeme));
        }

        [Fact]
        public void CanReceiveCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemeReceiver = new CatMemeReceiver();
            Assert.NotNull(() => catMemeReceiver.Handle(catMeme));
        }

        [Fact]
        public void SubscriberIsAddedToSubscribers()
        {
            var catMemeReceiver = new CatMemeReceiver();
            var catMemePublisher = new CatMemePublisher();
            catMemePublisher.Subscribe(catMemeReceiver);
            Assert.True(catMemePublisher.HasSubscriber(catMemeReceiver));
        }

        [Fact]
        public void SubscriberWillReceivePublishedCatMemes() // The only way I can think of confirming receipt is through the CatMemeReceiver's state
        {
            var catMemeReceivers = new List<CatMemeReceiver> { new CatMemeReceiver(), new CatMemeReceiver(), new CatMemeReceiver() };
            var catMemePublisher = new CatMemePublisher();
            foreach (var catMemeReceiver in catMemeReceivers)
            {
                catMemePublisher.Subscribe(catMemeReceiver);
            }
            var catMeme = new CatMeme();
            catMemePublisher.Publish(catMeme);
            foreach (var catMemeReceiver in catMemeReceivers)
            {
                Assert.NotEqual(1, catMemeReceiver.CatMemesReceived);
            }
        }

    }

    public interface IHandler<T>
    {
        void Handle(T t);
    };

    public class CatMeme
    {

    };
    public class CatMemeSender : IHandler<CatMeme>
    {
        public void Handle(CatMeme t)
        {
            return;
        }
    }

    public interface ISubscriber<T>
    {
        void Subscribe(T t);
    }
    public interface IPublisher<T>
    {
        void Publish(T t);
    }

    public class CatMemePublisher : ISubscriber<IHandler<CatMeme>>, IPublisher<CatMeme>
    {

        private List<IHandler<CatMeme>> _catMemeSubscribers = new List<IHandler<CatMeme>>();

        public bool HasSubscriber(IHandler<CatMeme> catMemeReciever)
        {
            return _catMemeSubscribers.Contains(catMemeReciever);
        }
        public void Publish(CatMeme catMeme)
        {
            foreach (var subscriber in _catMemeSubscribers)
            {
                subscriber.Handle(catMeme);
            }
        }

        public void Subscribe(IHandler<CatMeme> catMemeSubscriber)
        {
            _catMemeSubscribers.Add(catMemeSubscriber);
        }
    }

    public class CatMemeReceiver : IHandler<CatMeme>
    {

        private int _catMemesReceived = 0;

        public int CatMemesReceived => _catMemesReceived;

        public void Handle(CatMeme catMeme)
        {
            _catMemesReceived++;
        }
    }
}

