namespace PingAgain
{
    public class UnitTest1
    {

        // my job is to send cat memes --> Idk who is interested in cat memes, I just create the cat memes and send them;

        // THE TEST ABOVE AND BELOW ARE INDEPENDENTLY TESTABLE

        // people interested in cat memes sign up/subsribe to cat memes;
        // people who signed up for cat memes should receive anonymous cat memes --> They don't know who sent them;

        //BONUS ALL THE ABOVE BUT YOU CAN ALSO SEND DOG MEMES --> PEOPLE WHO SUBSCRIBE TO CAT RECEIVE CAT, PEOPLE WHO SUBSCRIBE TO DOG RECEIVE DOG AND PEOPLE WHO SUBSCRIBE TO BOTH RECEIVE DOG & CAT

        [Fact]
        public void SendPing()
        {

            var a = new Ping();
            var pinger = new Pinger();

            //send a ping
            // 
            //receive a pong
            Message m;
            pinger.Handle(a);
            m = null; // 

            Assert.IsType<Pong>(m);
        }

    }

    public interface IHandler<T> where T : Message
    {
        void Handle(T message);
    }
    class Pinger : IHandler<Ping>
    {
        //how do we say we can handle pings without specifying the method in the caller
        public void Handle(Ping message)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Message
    {

    }
    public class Ping : Message
    {

    }

    public class Pong : Message
    {

    }




}