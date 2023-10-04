
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
        public void SendCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemeSender = new CatMemeSender();
            catMemeSender.Handle(catMeme);
            Assert.NotNull(() => catMemeSender.Handle(catMeme));
        }

        [Fact]
        public void SubscribeToCatMemes()
        {
            var catMemeReceiver = new CatMemeReceiver();
            var catMemePublisher = new CatMemePublisher();
            Assert.NotNull(() => catMemePublisher.Subscribe(catMemeReceiver));
        }

        [Fact]
        public void PublishCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemePublisher = new CatMemePublisher();
            Assert.NotNull(() => catMemePublisher.Publish(catMeme));
        }

        [Fact]
        public void ReceiveCatMeme()
        {
            var catMeme = new CatMeme();
            var catMemeReceiver = new CatMemeReceiver();
            Assert.NotNull(() => catMemeReceiver.Handle(catMeme));
        }

    }

    public interface Handler<T>
    {
        void Handle(T t);
    };

    public class CatMeme
    {

    };
    public class CatMemeSender : Handler<CatMeme>
    {
        public void Handle(CatMeme t)
        {
            return;
        }
    }

    public interface Subscriber<T>
    {
        void Subscribe(T t);
    }
    public interface Publisher<T>
    {
        void Publish(T t);
    }

    public class CatMemePublisher : Subscriber<Handler<CatMeme>>, Publisher<CatMeme>
    {
        public void Publish(CatMeme t)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Handler<CatMeme> t)
        {
            return;
        }
    }

    public class CatMemeReceiver : Handler<CatMeme>
    {
        public void Handle(CatMeme t)
        {
            return;
        }
    }
}

