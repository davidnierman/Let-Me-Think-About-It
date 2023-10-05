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
            var dogAndCatmemeSender = new DogAndCatMemeSender();
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
            dogAndCatmemeSender.Subscribe(dogAndCatMemeRouter);
            dogAndCatMemeRouter.Subscribe(dogMemePublisher);
            dogAndCatMemeRouter.Subscribe(catMemePublisher);

            // Create Memes
            var catMeme = new CatMeme();
            var dogMeme = new DogMeme();

            // Send Memes
            //memeSender.Handle(catMeme);
            dogAndCatmemeSender.Handle(dogMeme);
            dogAndCatmemeSender.Handle(dogMeme);

            // Verify
            Assert.Equal(1, dogMemeReceiver.DogMemesReceived);
            Assert.Equal(1, catMemeReceiver.CatMemesReceived);
            Assert.Equal(2, dogAndCatMemeReceiver.MemesReceived);

        }
    }

    enum Animal
    {
        Cat,
        Dog
    }

    public interface IHandler<T>
    {
        void Handle(T t);
    };

    public interface ISubscriber<T> // this is technically a handler
    {
        void Subscribe(T subscriber);
    }
    public interface IPublisher<T> where T : Meme // this is technically a handler
    {
        void Publish(T t);
    }

    public abstract class Meme { }

    public class DogMeme : Meme
    {
        private readonly Animal _type = Animal.Dog;
    }

    public class CatMeme : Meme
    {
        private readonly Animal _type = Animal.Cat;
    };
    public class CatMemeSender : IHandler<Meme>, ISubscriber<IHandler<Meme>>
    {
        private IHandler<Meme>? _router;

        public void Handle(Meme catMeme)
        {
            _router?.Handle(catMeme);
        }


        public void Subscribe(IHandler<Meme> subscriber)
        {
            _router = subscriber;
        }

    }
    public class DogMemeSender : IHandler<Meme>, ISubscriber<IHandler<Meme>>
    {
        private IHandler<Meme>? _router;
        public void Handle(Meme dogMeme)
        {
            _router?.Handle(dogMeme);
        }

        public void Subscribe(IHandler<Meme> subscriber)
        {
            _router = subscriber;
        }

    }
    public class DogAndCatMemeSender : IHandler<Meme>, ISubscriber<IHandler<Meme>>
    {
        private IHandler<Meme>? _router;
        public void Handle(Meme dogMeme)
        {
            _router?.Handle(dogMeme);
        }

        public void Subscribe(IHandler<Meme> subscriber)
        {
            _router = subscriber;
        }
    }

    public class CatMemePublisher : ISubscriber<IHandler<Meme>>, IPublisher<Meme>, IHandler<Meme>, IHandler<CatMeme>
    {

        private List<IHandler<Meme>> _catMemeSubscribers = new List<IHandler<Meme>>();

        public void Handle(Meme t)
        {
            Publish(t);
        }

        public void Handle(CatMeme t)
        {
            Handle(t);
        }

        public bool HasSubscriber(IHandler<Meme> catMemeReciever)
        {
            return _catMemeSubscribers.Contains(catMemeReciever);
        }
        public void Publish(Meme catMeme)
        {
            foreach (var subscriber in _catMemeSubscribers)
            {
                subscriber.Handle(catMeme);
            }
        }

        public void Subscribe(IHandler<Meme> subscriber)
        {
            _catMemeSubscribers.Add(subscriber);
        }
    }
    public class DogMemePublisher : ISubscriber<IHandler<Meme>>, IPublisher<Meme>, IHandler<Meme>, IHandler<DogMeme> // what happens if it receives a generic meme???
    {

        private List<IHandler<Meme>> _dogMemeSubscribers = new List<IHandler<Meme>>();

        public void Handle(Meme t)
        {
            Publish(t);
        }

        public void Handle(DogMeme t)
        {
            Handle(t);
        }

        public bool HasSubscriber(IHandler<Meme> dogMemeReciever)
        {
            return _dogMemeSubscribers.Contains(dogMemeReciever);
        }
        public void Publish(Meme dogMeme)
        {
            foreach (var subscriber in _dogMemeSubscribers)
            {
                subscriber.Handle(dogMeme);
            }
        }

        public void Subscribe(IHandler<Meme> subscriber)
        {
            _dogMemeSubscribers.Add(subscriber);
        }
    }
    public class DogAndCatMemeRouter : IHandler<Meme>, ISubscriber<IHandler<CatMeme>>, ISubscriber<IHandler<DogMeme>> // how does the router know who can haandle what?
    {

        private List<IHandler<DogMeme>> _dogMemeHandlers = new List<IHandler<DogMeme>>();
        private List<IHandler<CatMeme>> _catMemeHandlers = new List<IHandler<CatMeme>>();

        public void Handle(Meme t)
        {
            var dogMeme = t as DogMeme;
            var catMeme = t as CatMeme;

            if (dogMeme != null)
            {
                foreach (var h in _dogMemeHandlers)
                {
                    h.Handle(dogMeme);
                }
            }
            else if (catMeme != null)
            {
                foreach (var h in _catMemeHandlers)
                {
                    h.Handle(catMeme);
                }
            }


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


    public class CatMemeReceiver : IHandler<Meme>
    {

        private int _catMemesReceived = 0;

        public int CatMemesReceived => _catMemesReceived;

        public void Handle(Meme meme)
        {
            var catMeme = meme as CatMeme;
            if (catMeme != null)
            {
                _catMemesReceived++;
            }
        }
    }
    public class DogMemeReceiver : IHandler<Meme>
    {

        private int _dogMemesReceived = 0;

        public int DogMemesReceived => _dogMemesReceived;

        public void Handle(Meme meme)
        {
            var dogMeme = meme as DogMeme;
            if (dogMeme != null)
            {
                _dogMemesReceived++;
            }
        }
    }
    public class DogAndCatMemeReceiver : IHandler<Meme>
    {

        private int _memesReceived = 0;

        public int MemesReceived => _memesReceived;

        public void Handle(Meme meme)
        {
            _memesReceived++;
        }

    }
}

