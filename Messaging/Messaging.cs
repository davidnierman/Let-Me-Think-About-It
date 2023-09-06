using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Xunit.Sdk;

namespace Messaging
{
    public class MesssaingTests
    {
        // [Fact]
        public void Create_Message()
        {
            var message = new Message("kitchen fire");
            Assert.NotNull(message.Content);
        }
        // [Fact]
        public void Create_Publisher()
        {
            var callerOne = new Caller("Dave");
            Assert.NotNull(callerOne);
        }

        // [Fact]
        public void Create_Handler()
        {
            Task callOfficer = new(() => Console.WriteLine("sending officers to the scene!"));
            var police = new Handler(callOfficer);
            police.Respond();
        }
        // [Fact]
        public void Create_Dispatcher()
        {
            Task callOfficer = new(() => Console.WriteLine("sending officers to the scene!"));
            var police = new Handler(callOfficer);
            Task callAmbulence = new(() => Console.WriteLine("sending paramedics!"));
            var hospital = new Handler(callAmbulence);
            Task callFireFighters = new(() => Console.WriteLine("sending a fire truck!"));
            var fireDepartment = new Handler(callFireFighters);
            var localDispatch = new Dispatcher(fireDepartment, hospital, police);
        }
        [Fact]
        public void Subscribe_Dispatcher()
        {
            var callerOne = new Caller("Dave");
            Assert.NotNull(callerOne);
            Task callOfficer = new(() => Console.WriteLine("sending officers to the scene!"));
            var police = new Handler(callOfficer);
            Task callAmbulence = new(() => Console.WriteLine("sending paramedics!"));
            var hospital = new Handler(callAmbulence);
            Task callFireFighters = new(() => Console.WriteLine("sending fire trucks!"));
            var fireDepartment = new Handler(callFireFighters);
            var localDispatch = new Dispatcher(fireDepartment, hospital, police);
            callerOne.Subscribe(localDispatch);
            callerOne.CallEmergencyLine("Fire!");
            callerOne.CallEmergencyLine("I need an ambulance");
            callerOne.CallEmergencyLine("Where are the police??");
        }
    }

    public class Message
    {
        private readonly string _content;

        public string Content => _content;

        public Message(string content)
        {
            _content = content;
        }
    }

    public class Caller : IPublisher // why is this not forcing implementation of subscribe???
    {
        private readonly string _name;

        public string Name => _name;

        public IPublisher.PublishMessage Subscriptions;

        public Caller(string name)
        {
            _name = name;
        }

        public void CallEmergencyLine(string recording)
        {
            var message = new Message(recording);
            Subscriptions.Invoke(message);
        }

        public void Subscribe(ISubscriber subscriber)
        {
            IPublisher.PublishMessage publish = subscriber.Listen;
            this.Subscriptions += publish;
        }
    }

    public interface IPublisher
    {

        public delegate void PublishMessage(Message message);

        public void Subscribe(ISubscriber subscriber);

    }

    public class Dispatcher : ISubscriber
    {
        private readonly Handler _fireDepartment;
        private readonly Handler _hospital;
        private readonly Handler _polceDepartment;

        public Dispatcher(Handler fireDepartment, Handler hospital, Handler policeDepartment)
        {
            _fireDepartment = fireDepartment;
            _hospital = hospital;
            _polceDepartment = policeDepartment;
        }

        public void Listen(Message message)
        {
            RouteCall(message);
        }

        public void RouteCall(Message message)
        {
            var lMessage = message.Content.ToLower();
            if (lMessage.Contains("fire"))
            {
                _fireDepartment.Respond();
            }
            if (lMessage.Contains("ambulance"))
            {
                _hospital.Respond();
            }
            if (lMessage.Contains("police"))
            {
                _polceDepartment.Respond();
            }
        }


    }

    public interface ISubscriber
    {
        public void Listen(Message message);
    }

    public class Handler
    {
        private readonly Task _acion;

        public void Respond()
        {
            _acion.Start();
        }
        public Handler(Task action)
        {
            _acion = action;
        }

    }
}