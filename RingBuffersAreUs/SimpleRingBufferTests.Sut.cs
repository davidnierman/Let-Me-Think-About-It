using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace RingBuffersAreUs
{
    partial class SimpleRingBufferTests
    {
        class TwoCentsRingBufferOriginal
        {
            private readonly char[] _buffer = new char[2];
            private int _head;
            private int _tail;

            public bool Write(char value)
            {
                var head = _head;
                if (head == _buffer.Length) return false;
                _buffer[head] = value;
                _head++;
                return true;
            }

            public bool Read(out char value)
            {
                value = default;
                //00 00, 01 00, 01 01, 10 00, 10 01, 10 10, 00 01
                //valid
                //01 00,10 00,10 01, 00 01
                if (_head < _tail || (_tail > 0 && _tail + 1 < _buffer.Length)) //check we can read
                {
                    return false;
                }

                value = _buffer[_tail];
                _tail++;
                if (_tail == _buffer.Length)
                {
                    _tail = 0;
                }

                if (_head == _buffer.Length)
                {
                    _head = 0;
                }

                return true;
            }
        }



        class TwoCentsRingBuffer
        {
            private readonly char[] _buffer = new char[2];
            private int _head;
            private int _tail;
            private int _count;
            public bool Write(char value)
            {
                if (_buffer.Length <= _count)
                    return false;
                _buffer[_head] = value;
                _count++;
                _head = (_head + 1) % _buffer.Length;
                return true;
            }

            public bool Read(out char value)
            {
                value = default;
                if (_count > 0)
                {
                    value = _buffer[_tail];
                    _tail = (_tail + 1) % _buffer.Length;
                    _count--;
                    return true;
                }
                return false;
            }

            public int Count()
            {
                return _count;
            }
        }
    }

    public class ZeroCentsBuffer
    {
        public bool Write(char value) => false;

        public bool Read(out char value)
        {
            value = default;
            return false;
        }
    }

    partial class LastResultsRingBuffer
    {
        class TwoCentsRingBuffer
        {
            private readonly char[] _buffer = new char[2];
            private int _head;
            private int _tail;
            private int _count;
            public bool Write(char value)
            {
                _buffer[_head] = value;
                _count = Math.Min(2, _count+1);
                _head = (_head + 1) % _buffer.Length;
                return true;
            }

            public bool Read(out char value)
            {
                value = default;
                if (_count > 0)
                {
                    value = _buffer[_tail];
                    _tail = (_tail + 1) % _buffer.Length;
                    _count--;
                    return true;
                }
                return false;
            }

            public int Count()
            {
                return _count;
            }
        }
    }

    partial class LastResultsRingBufferUsingAQueue
    {
        class TwoCentsRingBuffer
        {
            private int _max = 2;
            private ConcurrentQueue<char> _buffer = new();
            public bool Write(char value)
            {
                _buffer.Enqueue(value);
                while (_buffer.Count > _max + 10)
                {
                    _buffer.TryDequeue(out _);
                }
                return true;
            }

            public bool Read(out char value)
            {
                while(_buffer.Count > _max)
                {
                    _buffer.TryDequeue(out _);
                }
                return _buffer.TryDequeue(out value);
            }

            public int Count()
            {
                //while(_buffer.Count > _max)
                //{
                //    _buffer.TryDequeue(out _);
                //}
                return _buffer.Count;
            }
        }
    }

    public partial class LastResultsRingBufferUsingAChannel
    {
        class TwoCentsRingBuffer
        {
            private Channel<char> _channel = Channel.CreateBounded<char>(new BoundedChannelOptions(2)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.DropOldest
            });
            public bool Write(char c)
            {
                return _channel.Writer.TryWrite(c);
            }

            public bool Read(out char c)
            {
                return _channel.Reader.TryRead(out c);
            }

            public int Count()
            {
                return _channel.Reader.Count;
            }
        }
    }
}


