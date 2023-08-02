UnsafeStuff(new StringWriter());

using var sw = new StreamWriter("gcdemo.csv");
UnsafeStuff(sw);
sw.Flush();

void UnsafeStuff(TextWriter sw)
{
    unsafe
    {
        for (int i = 0; i < 100000; i++)
        {
            var o = new Foo();
            fixed(void* p = &o._a)
            {
                
                {
                    IntPtr ptr = (IntPtr)p;
                    sw.WriteLine($"{ptr.ToInt64()},");
                }
            }
        }
    }
}


class Foo
{
    public long _a;
    private long _b;
    private long _c;
    private long _d;
    private long _e;
    private long _f;
    private long _g;
}