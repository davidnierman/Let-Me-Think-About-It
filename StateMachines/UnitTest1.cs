using System.Runtime.InteropServices.ComTypes;

namespace StateMachines
{
    public class TurnstileTests
    {
        private readonly Turnstile _ts;

        public TurnstileTests()
        {
            _ts = new Turnstile();
        }
        [Fact]
        public void ShouldStartInTheLockedState()
        {
            
            Assert.Equal(Turnstile.TurnstileState.Locked, _ts.State);
        }

        [Fact]
        public void CannotPushInLockedState()
        {
            Assert.False(_ts.Push());
            Assert.Equal(Turnstile.TurnstileState.Locked, _ts.State);
            Assert.False(_ts.Push());
            Assert.Equal(Turnstile.TurnstileState.Locked, _ts.State);
            Assert.False(_ts.Push());
            Assert.Equal(Turnstile.TurnstileState.Locked, _ts.State);
        }

        [Fact]
        public void AddingCoinTransitionsToUnlocked()
        {
            _ts.Coin();
            Assert.Equal(Turnstile.TurnstileState.Unlocked, _ts.State);
            _ts.Coin();
            Assert.Equal(Turnstile.TurnstileState.Unlocked, _ts.State);
            _ts.Coin();
            Assert.Equal(Turnstile.TurnstileState.Unlocked, _ts.State);
        }

        [Fact]
        public void CanPushAnUnlockedTurnstile()
        {
            _ts.Coin();
            Assert.True(_ts.Push());
            Assert.Equal(Turnstile.TurnstileState.Locked, _ts.State);
        }

        class Turnstile
        {
            public TurnstileState State { get; private set; } = TurnstileState.Locked;
            public enum TurnstileState
            {
                Undefined,
                Locked,
                Unlocked
            }

            public bool Push()
            {
                var current = State;
                State = TurnstileState.Locked;
                return current == TurnstileState.Unlocked;
            }

            public void Coin()
            {
                State = TurnstileState.Unlocked;
            }
        }
    }

    public class PedestrianCrossingTestsV1
    {
        private readonly PedestrianCrossing _pc;

        public PedestrianCrossingTestsV1()
        {
            _pc = new PedestrianCrossing();
        }

        [Fact]
        public void StartsInOperationalState()
        {
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void OffShouldSwitchToOffline()
        {
            _pc.Off();
            Assert.Equal(PedestrianCrossing.OperationalStatus.Offline, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Red, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void ShouldFlashEverythingWhenOffline()
        {
            _pc.Off();
            for (int i = 0; i < 100; i++)
            {
                _pc.Timeout();
                Assert.Equal(PedestrianCrossing.OperationalStatus.Offline, _pc.Status);
                Assert.Equal(PedestrianCrossing.Cars.Blank, _pc.CarState);
                Assert.Equal(PedestrianCrossing.Pedestrian.Blank, _pc.PedestrianState);
                _pc.Timeout();
                Assert.Equal(PedestrianCrossing.OperationalStatus.Offline, _pc.Status);
                Assert.Equal(PedestrianCrossing.Cars.Red, _pc.CarState);
                Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
            }
        }

        [Fact]
        public void WhenOffSwitchingOnShouldReturnToOperationalState()
        {
            _pc.Off();
            _pc.On();

            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenFlashingAndSwitchingToOn()
        {
            _pc.Off();
            _pc.Timeout();
            _pc.On();

            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenCarsGreenBeforeTimeoutWhenPedestriansWaiting()
        {
            _pc.SignalPedestriansWaiting();
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenCarsGreenAfterTimeout()
        {
            _pc.Timeout();
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenTimeoutAfterCarsGreenAndPedestriansWaiting()
        {
            _pc.SignalPedestriansWaiting();
            _pc.Timeout();
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Yellow, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenPedestriansWaitingAfterCarsGreenAndTimeout()
        {
            _pc.Timeout();
            _pc.SignalPedestriansWaiting();
            
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Yellow, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenTimeoutAndCarsYellow()
        {
            _pc.Timeout();
            _pc.SignalPedestriansWaiting();
            _pc.Timeout();
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Red, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.Walk, _pc.PedestrianState);
        }

        [Fact]
        public void WhenPedestriansWalkAndTimeoutLessThanTen()
        {
            _pc.Timeout();
            _pc.SignalPedestriansWaiting();
            
            for (int i = 0; i < 4; i++)
            {
                _pc.Timeout();
                Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
                Assert.Equal(PedestrianCrossing.Cars.Red, _pc.CarState);
                Assert.Equal(PedestrianCrossing.Pedestrian.Walk, _pc.PedestrianState);
                _pc.Timeout();
                Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
                Assert.Equal(PedestrianCrossing.Cars.Red, _pc.CarState);
                Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
            }
        }

        [Fact]
        public void WhenPedestriansHaveRunAcrossTheRoad()
        {
            _pc.Timeout();
            _pc.SignalPedestriansWaiting();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();

            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        [Fact]
        public void Hmm()
        {
            _pc.SignalPedestriansWaiting();

            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            _pc.Timeout();
            
            Assert.Equal(PedestrianCrossing.OperationalStatus.Operational, _pc.Status);
            Assert.Equal(PedestrianCrossing.Cars.Green, _pc.CarState);
            Assert.Equal(PedestrianCrossing.Pedestrian.DoNotWalk, _pc.PedestrianState);
        }

        class PedestrianCrossing
        {
            private bool _timedOut = false;
            private bool _finishedWaitingForCars;
            private int _counter;
            private bool _pedestriansWaiting;
            public OperationalStatus Status { get; private set; }
            public Cars CarState { get; private set; }
            public Pedestrian PedestrianState { get; private set; }

            private bool PedestriansWaiting
            {
                get => _pedestriansWaiting;
                set => _pedestriansWaiting = value;
            }

            public PedestrianCrossing()
            {
                On();
            }
            public enum OperationalStatus
            {
                Undefined,
                Operational,
                Offline
            }

            public enum Cars
            {
                Undefined, 
                Red,
                Yellow,
                Green,
                Blank
            }

            public enum Pedestrian
            {
                Undefined,
                Walk,
                DoNotWalk,
                Blank
            }

            public void Off()
            {
                Status = OperationalStatus.Offline;
                CarState = Cars.Red;
            }

            public void On()
            {
                Status = OperationalStatus.Operational;
                CarState = Cars.Green;
                PedestrianState = Pedestrian.DoNotWalk;
            }

            public void Timeout()
            {
                if (Status == OperationalStatus.Offline)
                {
                    _timedOut = !_timedOut;
                    if (_timedOut)
                    {
                        CarState = Cars.Blank;
                        PedestrianState = Pedestrian.Blank;
                        return;
                    }
                    else
                    {
                        CarState = Cars.Red;
                        PedestrianState = Pedestrian.DoNotWalk;
                        return;
                    }
                }
                else
                {
                    if (PedestriansWaiting)
                    {
                        CarState = Cars.Yellow;
                        return;
                    }

                    if(CarState == Cars.Green)
                    {
                        _finishedWaitingForCars = true;
                        return;
                    }

                    if (CarState == Cars.Yellow)
                    {
                        CarState = Cars.Red;
                        PedestrianState = Pedestrian.Walk;
                        _counter = 0;
                        return;
                    }

                    _counter ++;
                    if (_counter == 9)
                    {
                        CarState = Cars.Green;
                        PedestrianState = Pedestrian.DoNotWalk;
                        _finishedWaitingForCars = false;
                        _pedestriansWaiting = false;
                        return;
                    }
                    if (PedestrianState == Pedestrian.Walk)
                    {
                        PedestrianState = Pedestrian.DoNotWalk;
                        return;
                    }

                    if (CarState == Cars.Red)
                    {
                        PedestrianState = Pedestrian.Walk;
                        return;
                    }
                }
            }

            public void SignalPedestriansWaiting()
            {
                if (_finishedWaitingForCars)
                {
                    CarState = Cars.Yellow;
                }
                else
                {
                    PedestriansWaiting = true;
                }
            }
        }
    }
}