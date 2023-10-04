namespace PingPong
{
    public class MailingTestsEnvelopes
    {

        [Fact]
        public void MachineStates()
        {
            var stateMachine = new Resident("some address");
            Assert.Equal(A.Active, stateMachine.State);
            stateMachine.Deactivate();
            Assert.Equal(A.Inactive, stateMachine.State);
            stateMachine.Deactivate();
            Assert.Equal(A.Inactive, stateMachine.State);
            stateMachine.Activate();
            Assert.Equal(A.Active, stateMachine.State);
            stateMachine.Activate();
            Assert.Equal(A.Active, stateMachine.State);
        }

        [Fact]
        public void CreateEnvelope()
        {
            var envelopeCreator = new EnvelopeCreator();
            string address = "Dave's Home";
            var resident = new Resident(address);
            envelopeCreator.CreateEnvelope(resident);
            Assert.Single(envelopeCreator.Outbox);
        }

        [Fact]
        public void SendEnvelope()
        {
            var envelopeCreator = new EnvelopeCreator();
            string address = "Dave's Home";
            var resident = new Resident(address);
            envelopeCreator.CreateEnvelope(resident);
            var envelopeCenter = new EnvelopeCenter();
            envelopeCreator.SendEnvelope(envelopeCenter);
            Assert.Empty(envelopeCreator.Outbox);
            Assert.Single(envelopeCenter.EnvelopeBox);
        }

        [Fact]
        public void DeliverEnvelope()
        {
            string address = "Dave's Home";
            var resident = new Resident(address);
            var envelopeCenter = new EnvelopeCenter();
            resident.SubscribeToEnvelopeDelivery(envelopeCenter);
            var envelopeCreator = new EnvelopeCreator();
            envelopeCreator.CreateEnvelope(resident);
            envelopeCreator.SendEnvelope(envelopeCenter);
            envelopeCenter.DeliverEnvelope();
            Assert.Single(resident.Inbox);
        }

        [Fact]
        public void ProcessEnvelopeMessageToChangeState()
        {
            string address = "Dave's Home";
            var resident = new Resident(address);
            var envelopeCenter = new EnvelopeCenter();
            resident.SubscribeToEnvelopeDelivery(envelopeCenter);
            var letterCreator = new LetterCreator();
            Action<Resident> deactivate = (Resident resident) => { resident.Deactivate(); };
            letterCreator.CreateActionLetter(resident, deactivate);
            letterCreator.SendEnvelope(envelopeCenter); // this is an envelope with a letter
            envelopeCenter.DeliverEnvelope();
            resident.ProcessEnvelope();
            Assert.Equal(A.Inactive, resident.State);
        }

    }

    [Flags]
    public enum A
    {
        Inactive = 0,
        Active = 1
    }
    public abstract class AThing
    {
        private A _state = A.Active;

        public A State => _state;

        public void Activate()
        {
            _state = A.Active;
        }

        public void Deactivate()
        {
            _state = A.Inactive;

        }
    } // I don't need this as an abstract. I need an interface to extract methods
    public class EnvelopeCreator : AThing
    {
        protected Queue<E> _outbox = new Queue<E>();

        public Queue<E> Outbox => _outbox;

        public void CreateEnvelope(Resident DestinationResident)
        {
            _outbox.Enqueue(new E(DestinationResident));
        }
        public void SendEnvelope(EnvelopeCenter envelopeCenter)
        {
            envelopeCenter.ReceiveMail(_outbox.Dequeue());
        }

    }
    public class LetterCreator : EnvelopeCreator
    {


        public void CreateActionLetter(Resident destinationResident, Action<Resident> todo)
        {
            _outbox.Enqueue(new ActionLetter(destinationResident, todo));
        }



    }
    public class EnvelopeCenter : AThing
    {
        private Queue<E> _envelopeBox = new Queue<E>();

        private List<Resident> _residents = new List<Resident>();

        public Queue<E> EnvelopeBox => _envelopeBox;

        public void ReceiveMail(E envelope)
        {
            _envelopeBox.Enqueue(envelope);
        }

        public void DeliverEnvelope()
        {
            foreach (var envelope in _envelopeBox)
            {
                foreach (var resident in _residents)
                {
                    if (envelope.DestinationResident == resident)
                    {
                        resident.ReceiveEnvelope(envelope);
                    }
                }
            }
        }

        public void AddResident(Resident resident)
        {
            _residents.Add(resident);
        }

    }
    public class Resident : AThing
    {
        private readonly string _address;

        private Queue<E> _inbox = new Queue<E>();

        public Queue<E> Inbox => _inbox;

        public Resident(string Address)
        {
            _address = Address;
        }

        public void SubscribeToEnvelopeDelivery(EnvelopeCenter envelopeCenter)
        {
            envelopeCenter.AddResident(this);
        }

        public void ReceiveEnvelope(E envelope)
        {
            _inbox.Enqueue(envelope);
        }

        public void ProcessEnvelope()
        {
            var envelope = _inbox.Dequeue();
            if (envelope is ActionLetter)
            {
                var letter = envelope as ActionLetter;
                ProcessActionLetter(letter);
            }
            else
            {
                return;
            }
        }

        public void ProcessActionLetter(ActionLetter letter)
        {
            var todo = letter.Todo as Action<Resident>;
            if (todo != null)
            {
                todo(this);
            }
        }


    }
    public class E
    {
        private readonly Resident _DestinationResident;

        public Resident DestinationResident => _DestinationResident;

        public E(Resident destinationResident)
        {
            _DestinationResident = destinationResident;
        }
    }
    public class ActionLetter : E
    {

        private readonly Action<Resident> _todo;

        public Action<Resident> Todo => _todo;

        public ActionLetter(Resident destinationResident, Action<Resident> todo) : base(destinationResident)
        {
            _todo = todo;

        }

    }

}