


using System.Diagnostics;
using Xunit.Abstractions;

namespace GarbageCollectionDemo
{
    public class GarbageCollectionDemoTests
    {

        readonly MemoryAllocator memoryAllocator = new();

        [Fact]
        public void RunMemoryAllocator()
        {
            using StreamWriter sw = new("../../../gcdemo.csv");
            sw.Flush();
            memoryAllocator.AllocateMemoryLoop(sw, 200000);
        }

    }

}
class MemoryAllocator
{
    public void AllocateMemoryLoop(TextWriter sw, int iterations)
    {
        sw.WriteLine($"memoryAddress,memoryUsage, garbageCollectionCount ");

        unsafe
        {
            long firstAddress = 0;
            GC.Collect();
            for (int i = 0; i < iterations; i++)
            {
                var o = new Foo();

                fixed (void* p = &o._a)
                {
                    IntPtr ptr = (IntPtr)p;
                    if (i == 0)
                    {
                        firstAddress = ptr.ToInt64(); // adding this hear to make the address number small enough for google sheets to chart

                    }
                    sw.WriteLine(value: $"{ptr.ToInt64() - firstAddress}, {GC.GetTotalMemory(false)}, {GC.CollectionCount(0) * 100000} "); // multiplying GC count so we can display it on a chart and actually see it on google sheets
                }
            }
        }
    }

}

class Foo
{
    public long _a;
}
