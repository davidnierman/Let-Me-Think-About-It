


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
            memoryAllocator.AllocateMemoryLoop(sw, 50000);
        }

    }


}
class MemoryAllocator
{
    public void AllocateMemoryLoop(TextWriter sw, int iterations)
    {
        unsafe
        {
            for (int i = 0; i < iterations; i++)
            {
                var o = new Foo();
                fixed (void* p = &o._a)
                {

                    {
                        IntPtr ptr = (IntPtr)p;
                        sw.WriteLine($"{ptr},");
                    }
                }
            }
        }
    }

}



class Foo
{
    public long _a;
}
