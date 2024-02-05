using Xunit.Abstractions;

namespace RingBuffersAreUs
{
    public partial class LastResultsRingBuffer
    {
        private readonly ITestOutputHelper _output;

        public LastResultsRingBuffer(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void CanWriteOneValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
        }

        [Fact]
        public void CanWriteMoreValuesThanCapacity()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void LastTwoValuesRemain()
        {
            var buffer = new TwoCentsRingBuffer();
            for (int i = 0; i < 26; i++)
            {
                Assert.True(buffer.Write((char)('A' + i)));    
                _output.WriteLine(new string((char)('A' + i), 1));
                if (i < 2)
                {
                    Assert.Equal(i+1, buffer.Count());
                }
                else
                {
                    Assert.Equal(2, buffer.Count());
                }
            }
            Assert.True(buffer.Read(out var c));
            Assert.Equal('Y' , c);
            Assert.True(buffer.Read(out c));
            Assert.Equal('Z' , c);
        }

        [Fact]
        public void ReadingEmptyBufferShouldReturnFalse()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
        }

        [Fact]
        public void CanReadWrittenValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void CanFillBufferAndReadAllValuesBack()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var read));
            Assert.Equal('A', read);
            Assert.True(buffer.Read(out read));
            Assert.Equal('B', read);
        }

        [Fact]
        public void ReadReadWriteReadShouldPass()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
            Assert.False(buffer.Read(out _));
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void WriteWriteReadWrite()
        {
            //Two trailer park girls go round the outside
            //Reset the head to the beginning / chase the tail
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void WriteWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Read(out value));//h0t2
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h0t1 -> h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t1
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
            Assert.True(buffer.Read(out value));
            Assert.Equal('C', value);
        }

        [Fact]
        public void WriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Read(out var value));//h1t0 -> h1t1
            Assert.Equal('A', value);
            Assert.False(buffer.Read(out value));//h1t1
        }

        [Fact]
        public void CountShouldBeCorrect()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.Equal(0, buffer.Count());
            buffer.Write('A');
            Assert.Equal(1, buffer.Count());
            buffer.Write('B');
            Assert.Equal(2, buffer.Count());
        }
    }

    public partial class LastResultsRingBufferUsingAChannel
    {
        private readonly ITestOutputHelper _output;

        public LastResultsRingBufferUsingAChannel(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void CanWriteOneValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
        }

        [Fact]
        public void CanWriteMoreValuesThanCapacity()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void LastTwoValuesRemain()
        {
            var buffer = new TwoCentsRingBuffer();
            for (int i = 0; i < 26; i++)
            {
                Assert.True(buffer.Write((char)('A' + i)));    
                _output.WriteLine(new string((char)('A' + i), 1));
                //if (i < 2)
                //{
                //    Assert.Equal(i+1, buffer.Count());
                //}
                //else
                //{
                //    Assert.Equal(2, buffer.Count());
                //}
            }
            Assert.True(buffer.Read(out var c));
            Assert.Equal('Y' , c);
            Assert.True(buffer.Read(out c));
            Assert.Equal('Z' , c);
        }

        [Fact]
        public void ReadingEmptyBufferShouldReturnFalse()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
        }

        [Fact]
        public void CanReadWrittenValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void CanFillBufferAndReadAllValuesBack()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var read));
            Assert.Equal('A', read);
            Assert.True(buffer.Read(out read));
            Assert.Equal('B', read);
        }

        [Fact]
        public void ReadReadWriteReadShouldPass()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
            Assert.False(buffer.Read(out _));
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void WriteWriteReadWrite()
        {
            //Two trailer park girls go round the outside
            //Reset the head to the beginning / chase the tail
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void WriteWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Read(out value));//h0t2
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h0t1 -> h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t1
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
            Assert.True(buffer.Read(out value));
            Assert.Equal('C', value);
        }

        [Fact]
        public void WriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Read(out var value));//h1t0 -> h1t1
            Assert.Equal('A', value);
            Assert.False(buffer.Read(out value));//h1t1
        }

        [Fact]
        public void CountShouldBeCorrect()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.Equal(0, buffer.Count());
            buffer.Write('A');
            Assert.Equal(1, buffer.Count());
            buffer.Write('B');
            Assert.Equal(2, buffer.Count());
        }
    }

    public partial class LastResultsRingBufferUsingAQueue
    {
        private readonly ITestOutputHelper _output;

        public LastResultsRingBufferUsingAQueue(ITestOutputHelper output)
        {
            _output = output;
        }
        [Fact]
        public void CanWriteOneValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
        }

        [Fact]
        public void CanWriteMoreValuesThanCapacity()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void LastTwoValuesRemain()
        {
            var buffer = new TwoCentsRingBuffer();
            for (int i = 0; i < 26; i++)
            {
                Assert.True(buffer.Write((char)('A' + i)));    
                _output.WriteLine(new string((char)('A' + i), 1));
                //if (i < 2)
                //{
                //    Assert.Equal(i+1, buffer.Count());
                //}
                //else
                //{
                //    Assert.Equal(2, buffer.Count());
                //}
            }
            Assert.True(buffer.Read(out var c));
            Assert.Equal('Y' , c);
            Assert.True(buffer.Read(out c));
            Assert.Equal('Z' , c);
        }

        [Fact]
        public void ReadingEmptyBufferShouldReturnFalse()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
        }

        [Fact]
        public void CanReadWrittenValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void CanFillBufferAndReadAllValuesBack()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var read));
            Assert.Equal('A', read);
            Assert.True(buffer.Read(out read));
            Assert.Equal('B', read);
        }

        [Fact]
        public void ReadReadWriteReadShouldPass()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
            Assert.False(buffer.Read(out _));
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void WriteWriteReadWrite()
        {
            //Two trailer park girls go round the outside
            //Reset the head to the beginning / chase the tail
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void WriteWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Read(out value));//h0t2
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h0t1 -> h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t1
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
            Assert.True(buffer.Read(out value));
            Assert.Equal('C', value);
        }

        [Fact]
        public void WriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Read(out var value));//h1t0 -> h1t1
            Assert.Equal('A', value);
            Assert.False(buffer.Read(out value));//h1t1
        }

        [Fact]
        public void CountShouldBeCorrect()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.Equal(0, buffer.Count());
            buffer.Write('A');
            Assert.Equal(1, buffer.Count());
            buffer.Write('B');
            Assert.Equal(2, buffer.Count());
        }
    }
    public partial class SimpleRingBufferTests
    {
        [Fact]
        public void CanWriteOneValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
        }

        [Fact]
        public void CannotWriteMoreValuesThanCapacity()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.False(buffer.Write('C'));
        }

        [Fact]
        public void ReadingEmptyBufferShouldReturnFalse()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
        }

        [Fact]
        public void CanReadWrittenValue()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void CanFillBufferAndReadAllValuesBack()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var read));
            Assert.Equal('A', read);
            Assert.True(buffer.Read(out read));
            Assert.Equal('B', read);
        }

        [Fact]
        public void ReadReadWriteReadShouldPass()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.False(buffer.Read(out _));
            Assert.False(buffer.Read(out _));
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
        }

        [Fact]
        public void WriteWriteReadWrite()
        {
            //Two trailer park girls go round the outside
            //Reset the head to the beginning / chase the tail
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A'));
            Assert.True(buffer.Write('B'));
            Assert.True(buffer.Read(out var value));
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));
        }

        [Fact]
        public void WriteWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Read(out value));//h0t2
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t0
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h0t1 -> h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
        }

        [Fact]
        public void WriteWriteReadWriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Write('B'));// h2t1
            Assert.True(buffer.Read(out var value));//h0t1
            Assert.Equal('A', value);
            Assert.True(buffer.Write('C'));//h1t1
            Assert.True(buffer.Read(out value));
            Assert.Equal('B', value);
            Assert.True(buffer.Read(out value));
            Assert.Equal('C', value);
        }

        [Fact]
        public void WriteReadRead()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.True(buffer.Write('A')); // h1t0
            Assert.True(buffer.Read(out var value));//h1t0 -> h1t1
            Assert.Equal('A', value);
            Assert.False(buffer.Read(out value));//h1t1
        }

        [Fact]
        public void CountShouldBeCorrect()
        {
            var buffer = new TwoCentsRingBuffer();
            Assert.Equal(0, buffer.Count());
            buffer.Write('A');
            Assert.Equal(1, buffer.Count());
            buffer.Write('B');
            Assert.Equal(2, buffer.Count());
        }

        //[Fact]
        //public void Foo()
        //{
        //    // 0000 0000    1000 0000
        //    // 0000 0001    1000 0001
        //    var (h, t) = (0, -1);
        //    Assert.Equal(0, (h < 0 ? 0 : h) - (t > 0 ? t : 0));
        //    (h, t) = (1, -1);
        //    Assert.Equal(1, (h > 0 ? 2 : h) - (t > 0 ? t : 0));
        //    (h, t) = (0, ~0);
        //    Assert.Equal(2, ~(h - t));
        //}
    }

    
}