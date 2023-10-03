namespace PingPong
{
    public class MailingTestsEnvelopes
    {

        [Fact]
        public void MachineStates()
        {
            var stateMachine = new Resident("some address");
            Assert.Equal(MachineState.Active, stateMachine.State);
            stateMachine.Deactivate();
            Assert.Equal(MachineState.Inactive, stateMachine.State);
            stateMachine.Deactivate();
            Assert.Equal(MachineState.Inactive, stateMachine.State);
            stateMachine.Activate();
            Assert.Equal(MachineState.Active, stateMachine.State);
            stateMachine.Activate();
            Assert.Equal(MachineState.Active, stateMachine.State);
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
            Assert.Equal(MachineState.Inactive, resident.State);
        }

    }

    [Flags]
    public enum MachineState
    {
        Inactive = 0,
        Active = 1
    }
    public abstract class StateMachine
    {
        private MachineState _state = MachineState.Active;

        public MachineState State => _state;

        public void Activate()
        {
            _state = MachineState.Active;
        }

        public void Deactivate()
        {
            _state = MachineState.Inactive;

        }
    }
    public class EnvelopeCreator : StateMachine
    {
        protected Queue<Envelope> _outbox = new Queue<Envelope>();

        public Queue<Envelope> Outbox => _outbox;

        public void CreateEnvelope(Resident DestinationResident)
        {
            _outbox.Enqueue(new Envelope(DestinationResident));
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
    public class EnvelopeCenter : StateMachine
    {
        private Queue<Envelope> _envelopeBox = new Queue<Envelope>();

        private List<Resident> _residents = new List<Resident>();

        public Queue<Envelope> EnvelopeBox => _envelopeBox;

        public void ReceiveMail(Envelope envelope)
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
    public class Resident : StateMachine
    {
        private readonly string _address;

        private Queue<Envelope> _inbox = new Queue<Envelope>();

        public Queue<Envelope> Inbox => _inbox;

        public Resident(string Address)
        {
            _address = Address;
        }

        public void SubscribeToEnvelopeDelivery(EnvelopeCenter envelopeCenter)
        {
            envelopeCenter.AddResident(this);
        }

        public void ReceiveEnvelope(Envelope envelope)
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
    public class Envelope
    {
        private readonly Resident _DestinationResident;

        public Resident DestinationResident => _DestinationResident;

        public Envelope(Resident destinationResident)
        {
            _DestinationResident = destinationResident;
        }
    }
    public class ActionLetter : Envelope
    {

        private readonly Action<Resident> _todo;

        public Action<Resident> Todo => _todo;

        public ActionLetter(Resident destinationResident, Action<Resident> todo) : base(destinationResident)
        {
            _todo = todo;

        }

    }
}