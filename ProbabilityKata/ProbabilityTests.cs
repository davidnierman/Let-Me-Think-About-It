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

        public static IEnumerable<object[]> GetProbabilityValues()
        {
            for (decimal i = 0; i <= 1m; i+=0.1m)
            {
                yield return new object[] { i };
            }
            
        }
        
        [Theory]
        [MemberData(nameof(GetProbabilityValues))]
        public void InverseGivesTheCorrectAnswer(decimal value)
        {
            var p1 = new Probability(value);
            var p2 = p1.Inverse();
            if (!p1.Equals(new Probability(0.5m)))
            {
                Assert.NotEqual(p1, p2);
            }
            else
            {
                Assert.Equal(p1, p2);
            }

            Assert.Equal(p1, p2.Inverse());
        }

        [Fact]
        public void Combine()
        {
            var p1 = new Probability(0.1m);
            var p2 = new Probability(0.1m);
            var expected = new Probability(0.01m); // P(a).P(b)

            Assert.Equal(expected, p1.CombinedWith(p2));
        }

        [Fact]
        public void Either() //P(A) + P(B) - P(A)*P(B)
        {
            var pA = new Probability(0.1m).CombinedWith(new Probability(0.1m)); // 5 followed by 1 == 0.01 0.1 * 0.1 = 0.01
            var pB = new Probability(0.1m).CombinedWith(new Probability(0.1m)); // 6 followed by 3 == 0.01
            // 0.01 * 0.01 = 0.0001
            // 5 or 6 == 0.02 -            0.0001
            var expected = new Probability(0.0199m);

            Assert.Equal(expected, pA.Either(pB));
        }
    }

    class Probability
    {
        private readonly decimal _value;

        public Probability(decimal value)
        {
            _value = value;
        }

        public Probability Either(Probability other)
        {
            return new Probability(_value + other._value -  _value * other._value);
        }

        public Probability CombinedWith(Probability other)
        {
            return new Probability(_value * other._value);
        }

        public Probability Inverse()
        {
            return new Probability(1 - _value);
        }

        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Probability p) return false;
            return this._value == p._value;
        }

        public override string ToString()
        {
            return $"{_value}";
        }
    }

}