namespace ProbabilityKata
{
    public class ProbabilityTests
    {
        [Fact]
        public void TwoProbabilitiesWithTheSameValueShouldBeEqual()
        {
            var p1 = new Probability(0.5m);
            var p2 = new Probability(0.5m);
            Assert.Equal(p1, p2);
        }

        [Fact]
        public void TwoProbabilitiesWithTheDifferentValuesShouldNotBeEqual()
        {
            var p1 = new Probability(0.5m);
            var p2 = new Probability(0.1m);
            Assert.NotEqual(p1, p2);
        }
    }

    class Probability
    {
        private readonly decimal _value;

        public Probability(decimal value)
        {
            _value = value;
        }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Probability p) return false;
            return this._value == p._value;
        }
        
    }

}