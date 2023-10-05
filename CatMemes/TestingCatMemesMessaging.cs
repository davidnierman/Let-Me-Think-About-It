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
            Assert.NotNull(() => catMemePublisher.Handle(catMeme));
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
            var catMemeSender = new CatMemeSender();
            catMemeSender.Subscribe(catMemePublisher);
            var catMeme = new CatMeme();
            catMemeSender.Handle(catMeme);
            foreach (var catMemeReceiver in catMemeReceivers)
            {
                Assert.NotEqual(0, catMemeReceiver.CatMemesReceived);
            }
        }

        [Fact]
        public void SubscriberWillReceivePublishedDogMemes() // The only way I can think of confirming receipt is through the DogMemeReceiver's state
        {
            var dogMemeReceivers = new List<DogMemeReceiver> { new DogMemeReceiver(), new DogMemeReceiver(), new DogMemeReceiver() };
            var dogMemePublisher = new DogMemePublisher();
            foreach (var dogMemeReceiver in dogMemeReceivers)
            {
                dogMemePublisher.Subscribe(dogMemeReceiver);
            }
            var dogMemeSender = new DogMemeSender();
            dogMemeSender.Subscribe(dogMemePublisher);
            var dogMeme = new DogMeme();
            dogMemeSender.Handle(dogMeme);
            foreach (var dogMemeReceiver in dogMemeReceivers)
            {
                Assert.NotEqual(0, dogMemeReceiver.DogMemesReceived);
            }
        }

        [Fact]
        public void DogSubScribersOnlyReceiveDogMemes()
        {
            var dogMemeReceivers = new List<DogMemeReceiver> { new DogMemeReceiver(), new DogMemeReceiver(), new DogMemeReceiver() };
            var dogMemePublisher = new DogMemePublisher();

            foreach (var dogMemeReceiver in dogMemeReceivers)
            {
                dogMemePublisher.Subscribe(dogMemeReceiver);
            }

            var catMemeSender = new CatMemeSender();
            var catMemePublisher = new CatMemePublisher();
            catMemeSender.Subscribe(catMemePublisher);
            var catMeme = new CatMeme();
            catMemeSender.Handle(catMeme);


            foreach (var dogMemeReceiver in dogMemeReceivers)
            {
                Assert.Equal(0, dogMemeReceiver.DogMemesReceived);
            }
        }

        [Fact]
        public void CatSubScribersOnlyReceiveCatMemes()
        {
            var catMemeReceivers = new List<CatMemeReceiver> { new CatMemeReceiver(), new CatMemeReceiver(), new CatMemeReceiver() };
            var catMemePublisher = new CatMemePublisher();

            foreach (var dogMemeReceiver in catMemeReceivers)
            {
                catMemePublisher.Subscribe(dogMemeReceiver);
            }

            var dogMemeSender = new DogMemeSender();
            var dogMemePublisher = new DogMemePublisher();
            dogMemeSender.Subscribe(dogMemePublisher);
            var dogMeme = new DogMeme();
            dogMemeSender.Handle(dogMeme);

            foreach (var catMemeReceiver in catMemeReceivers)
            {
                Assert.Equal(0, catMemeReceiver.CatMemesReceived);
            }
        }

        [Fact]
        public void CatDogSubScribersBothReceiveCatAndDogMemes()
        {
            var dogAndCatMemeReceivers = new List<DogAndCatMemeReceiver> { new DogAndCatMemeReceiver(), new DogAndCatMemeReceiver(), new DogAndCatMemeReceiver() };
            var catMemePublisher = new CatMemePublisher();
            var dogMemePublisher = new DogMemePublisher();

            foreach (var dogAndCatMemeReceiver in dogAndCatMemeReceivers)
            {
                catMemePublisher.Subscribe(dogAndCatMemeReceiver);
                dogMemePublisher.Subscribe(dogAndCatMemeReceiver);
            }

            var dogMemeSender = new DogMemeSender();
            dogMemeSender.Subscribe(dogMemePublisher);
            var dogMeme = new DogMeme();
            dogMemeSender.Handle(dogMeme);

            var catMemeSender = new CatMemeSender();
            catMemeSender.Subscribe(catMemePublisher);
            var catMeme = new CatMeme();
            catMemeSender.Handle(catMeme);
            catMemeSender.Handle(catMeme);

            foreach (var dogAndCatMemeReceiver in dogAndCatMemeReceivers)
            {
                Assert.Equal(3, dogAndCatMemeReceiver.MemesReceived);
            }
        }


        [Fact]
        public void CatAndDogSenderCanSendMemesToCatAndDogSubscribers()
        {
            // Meme Forwarders
            var dogMemeSender = new DogMemeSender();
            var catMemeSender = new CatMemeSender();
            var dogAndCatMemeRouter = new DogAndCatMemeRouter();
            var dogMemePublisher = new DogMemePublisher();
            var catMemePublisher = new CatMemePublisher();

            // Meme Receivers
            var dogMemeReceiver = new DogMemeReceiver();
            var catMemeReceiver = new CatMemeReceiver();
            var dogAndCatMemeReceiver = new DogAndCatMemeReceiver();

            // Subscribe the Meme Forwarders
            dogMemePublisher.Subscribe(dogMemeReceiver);
            dogMemePublisher.Subscribe(dogAndCatMemeReceiver);
            catMemePublisher.Subscribe(catMemeReceiver);
            catMemePublisher.Subscribe(dogAndCatMemeReceiver);
            dogMemeSender.Subscribe(dogAndCatMemeRouter);
            catMemeSender.Subscribe(dogAndCatMemeRouter);
            dogAndCatMemeRouter.Subscribe(dogMemePublisher);
            dogAndCatMemeRouter.Subscribe(catMemePublisher);

            // Create Memes
            var catMeme = new CatMeme();
            var dogMeme = new DogMeme();

            // Send Memes
            dogMemeSender.Handle(dogMeme);
            catMemeSender.Handle(catMeme);

            // Verify
            Assert.Equal(1, dogMemeReceiver.DogMemesReceived);
            Assert.Equal(1, catMemeReceiver.CatMemesReceived);
            Assert.Equal(2, dogAndCatMemeReceiver.MemesReceived);
        }
    }
    public interface IHandler<T>
    {
        void Handle(T t);
    };

    public interface ISubscriber<T> // why can't I limit where T: IHandler<T> ? // this is technically a handler that handles a subscriber/Handler
    {
        void Subscribe(T subscriber);
    }
    public interface IPublisher<T> where T : Meme // this is technically a handler that handles a Meme
    {
        void Publish(T t);
    }

    public abstract class Meme { }

    public class DogMeme : Meme
    {

    }

    public class CatMeme : Meme
    {

    };
    public class CatMemeSender : IHandler<CatMeme>, ISubscriber<IHandler<CatMeme>>
    {
        private IHandler<CatMeme>? _router;

        public void Handle(CatMeme catMeme)
        {
            _router?.Handle(catMeme);
        }


        public void Subscribe(IHandler<CatMeme> subscriber)
        {
            _router = subscriber;
        }

    }
    public class DogMemeSender : IHandler<DogMeme>, ISubscriber<IHandler<DogMeme>>
    {
        private IHandler<DogMeme>? _router;
        public void Handle(DogMeme dogMeme)
        {
            _router?.Handle(dogMeme);
        }

        public void Subscribe(IHandler<DogMeme> subscriber)
        {
            _router = subscriber;
        }

    }

    public class CatMemePublisher : ISubscriber<IHandler<CatMeme>>, IPublisher<CatMeme>, IHandler<CatMeme>
    {

        private List<IHandler<CatMeme>> _catMemeSubscribers = new List<IHandler<CatMeme>>();


        public void Handle(CatMeme t)
        {
            Publish(t);
        }

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

        public void Subscribe(IHandler<CatMeme> subscriber)
        {
            _catMemeSubscribers.Add(subscriber);
        }
    }
    public class DogMemePublisher : ISubscriber<IHandler<DogMeme>>, IPublisher<DogMeme>, IHandler<DogMeme>
    {

        private List<IHandler<DogMeme>> _dogMemeSubscribers = new List<IHandler<DogMeme>>();


        public void Handle(DogMeme t)
        {
            Publish(t);
        }

        public bool HasSubscriber(IHandler<DogMeme> dogMemeReciever)
        {
            return _dogMemeSubscribers.Contains(dogMemeReciever);
        }
        public void Publish(DogMeme dogMeme)
        {
            foreach (var subscriber in _dogMemeSubscribers)
            {
                subscriber.Handle(dogMeme);
            }
        }

        public void Subscribe(IHandler<DogMeme> subscriber)
        {
            _dogMemeSubscribers.Add(subscriber);
        }
    }
    public class DogAndCatMemeRouter : IHandler<CatMeme>, IHandler<DogMeme>, ISubscriber<IHandler<CatMeme>>, ISubscriber<IHandler<DogMeme>>
    {

        private List<IHandler<DogMeme>> _dogMemeHandlers = new List<IHandler<DogMeme>>();
        private List<IHandler<CatMeme>> _catMemeHandlers = new List<IHandler<CatMeme>>();

        public void Handle(CatMeme t)
        {
            foreach (var handler in _catMemeHandlers) { handler.Handle(t); }
        }

        public void Handle(DogMeme t)
        {
            foreach (var handler in _dogMemeHandlers) { handler.Handle(t); }
        }

        public void Subscribe(IHandler<CatMeme> subscriber)
        {
            _catMemeHandlers.Add(subscriber);
        }

        public void Subscribe(IHandler<DogMeme> subscriber)
        {
            _dogMemeHandlers.Add(subscriber);
        }
    }

    public class CatMemeReceiver : IHandler<CatMeme>
    {

        private int _catMemesReceived = 0;

        public int CatMemesReceived => _catMemesReceived;

        public void Handle(CatMeme meme)
        {
            var catMeme = meme as CatMeme;
            if (catMeme != null)
            {
                _catMemesReceived++;
            }
        }
    }
    public class DogMemeReceiver : IHandler<DogMeme>
    {

        private int _dogMemesReceived = 0;

        public int DogMemesReceived => _dogMemesReceived;

        public void Handle(DogMeme meme)
        {
            var dogMeme = meme as DogMeme;
            if (dogMeme != null)
            {
                _dogMemesReceived++;
            }
        }
    }
    public class DogAndCatMemeReceiver : IHandler<CatMeme>, IHandler<DogMeme>
    {

        private int _memesReceived = 0;

        public int MemesReceived => _memesReceived;

        public void Handle(CatMeme meme)
        {
            _memesReceived++;
        }
        public void Handle(DogMeme meme)
        {
            _memesReceived++;
        }

    }
}

