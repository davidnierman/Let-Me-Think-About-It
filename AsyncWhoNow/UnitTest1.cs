using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace AsyncWhoNow;

public class AsyncCoordination
{
    private readonly ITestOutputHelper _output;
    private readonly (int eventNumber,int eventData)[] _a = [(0,1), (0,3), (0,2), (1,4), (0,5), (2,6), (3,8), (0,7), (0,9), (4,42)];
    private readonly (int eventNumber,int eventData)[] _b = [(0,1), (0,2), (0,3), (1,4), (0,5), (2,6), (0,7), (3,8), (0,9), (4,42)];
    private readonly (int eventNumber,int eventData)[] _d = [(0, 1)];
    public AsyncCoordination(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public void CheckSequenceEqual()
    {
        var filteredA = _a.Where(x => x.eventData % 2 == 0);

        var filteredB = _b.Where(x => x.eventData % 2 == 0);

        var aEnumerator = filteredA.GetEnumerator();
        var bEnumerator = filteredB.GetEnumerator();

        if (aEnumerator.MoveNext())
        {
            Assert.True(bEnumerator.MoveNext());
            Assert.Equal(aEnumerator.Current.eventNumber, bEnumerator.Current.eventNumber);
            Assert.Equal(aEnumerator.Current.eventData, bEnumerator.Current.eventData);
        }
    }
 
    [Fact]
    public async Task CheckCallbacksSequenceEqual()
    {
        var filteredA = _a.Where(x => x.eventData % 2 == 0);
        

        var filteredB = _b.Where(x => x.eventData % 2 == 0);

        var aEnumerator = filteredA.GetEnumerator();
        var bEnumerator = filteredB.GetEnumerator();

        TaskCompletionSource<(int i, int x)> sourceArrived = new TaskCompletionSource<(int i, int x)>();
        TaskCompletionSource readyPlayerOne = new();
        //readyPlayerOne.TrySetResult();

        var t1 = Subscribe(aEnumerator, HandleA, 500);
        var t2 = Subscribe(bEnumerator, HandleB, 100);
        await Task.WhenAll(t1, t2);

        async Task HandleA((int i, int x) e)
        {
            sourceArrived.TrySetResult(e);
            await readyPlayerOne.Task;
            readyPlayerOne = new();
        }

        async Task HandleB((int i, int x) e)
        {
            var sourceEvent = await sourceArrived.Task;
            sourceArrived = new TaskCompletionSource<(int i, int x)>();
            readyPlayerOne.TrySetResult();
            Assert.Equal(sourceEvent.i, e.i);
            Assert.Equal(sourceEvent.x, e.x);
        }

        return;
        async Task Subscribe(IEnumerator<(int i, int x)> enumerator, Func<(int i, int x), Task> handler, int delay)
        {
            while (enumerator.MoveNext())
            {
                await Task.Delay(delay);
                await handler(enumerator.Current);
            }
        }
    }

    [Fact]
    public async Task CheckCallbacksSequenceUnfilteredEqual()
    {
        var filteredA = _a.Select(x=>x);
        

        var filteredB = _b.Select(x=>x);

        var aEnumerator = filteredA.GetEnumerator();
        var bEnumerator = filteredB.GetEnumerator();

        TaskCompletionSource<(int i, int x)> sourceArrived = new TaskCompletionSource<(int i, int x)>();
        TaskCompletionSource readyPlayerOne = new();
        var mres = new ManualResetEventSlim(false);
        _ = Subscribe(aEnumerator, HandleA, 50, () => mres.Set());
        var t2 = Subscribe(bEnumerator, HandleB, 10);
        
        await t2.WaitAsync(TimeSpan.FromMilliseconds(1000));
        Assert.True(mres.Wait(1000), "Missing events in destination");

        async Task HandleA((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            sourceArrived.TrySetResult(e);
            await readyPlayerOne.Task;
            readyPlayerOne = new();
        }

        async Task HandleB((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            var sourceEvent = await sourceArrived.Task;
            sourceArrived = new TaskCompletionSource<(int i, int x)>();
            readyPlayerOne.TrySetResult();
            Assert.Equal(sourceEvent.i, e.i);
            Assert.Equal(sourceEvent.x, e.x);
        }

        return;
        async Task Subscribe(IEnumerator<(int eventNumber, int eventData)> enumerator, Func<(int i, int x), Task> handler, int delay,
                             Action? onLive = null)
        {
            while (enumerator.MoveNext())
            {
                await Task.Delay(delay);
                await handler(enumerator.Current);
            }
            onLive?.Invoke();
        }
    }

    [Fact]
    public async Task CheckCallbacksUnbalanced()
    {
        var filteredA = _a.Select(x=>x);
        

        var filteredB = _d.Select(x=>x);

        var aEnumerator = filteredA.GetEnumerator();
        var bEnumerator = filteredB.GetEnumerator();

        TaskCompletionSource<(int i, int x)> sourceArrived = new TaskCompletionSource<(int i, int x)>();
        TaskCompletionSource readyPlayerOne = new();
        //readyPlayerOne.TrySetResult();


        var t1 = Subscribe(aEnumerator, HandleA,10);
        var t2 = Subscribe(bEnumerator, HandleB,50);
        await t2;
        //Unbalanced list check
        var ex = await Record.ExceptionAsync(()=> t1.WaitAsync(TimeSpan.FromSeconds(1)));
        Assert.IsType<TimeoutException>(ex);

        async Task HandleA((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            sourceArrived.TrySetResult(e);
            await readyPlayerOne.Task;
            readyPlayerOne = new();
        }

        async Task HandleB((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            var sourceEvent = await sourceArrived.Task;
            sourceArrived = new TaskCompletionSource<(int i, int x)>();
            readyPlayerOne.TrySetResult();
            Assert.Equal(sourceEvent.i, e.i);
            Assert.Equal(sourceEvent.x, e.x);
        }

        return;
        async Task Subscribe(IEnumerator<(int eventNumber, int eventData)> enumerator, 
                             Func<(int i, int x), Task> handler, 
                             int delay)
        {
            while (enumerator.MoveNext())
            {
                await Task.Delay(delay);
                await handler(enumerator.Current);
            }
        }
    }

    [Fact]
    public async Task CheckCallbacksUnbalancedMRES()
    {
        var filteredA = _a.Select(x=>x);
        

        var filteredB = _d.Select(x=>x);

        var aEnumerator = filteredA.GetEnumerator();
        var bEnumerator = filteredB.GetEnumerator();

        TaskCompletionSource<(int i, int x)> sourceArrived = new TaskCompletionSource<(int i, int x)>();
        TaskCompletionSource readyPlayerOne = new();
        //readyPlayerOne.TrySetResult();
        var mres = new ManualResetEventSlim(false);

        _ = Subscribe(aEnumerator, HandleA,10, () => mres.Set());
        var t2 = Subscribe(bEnumerator, HandleB,50);
        await t2.WaitAsync(TimeSpan.FromSeconds(3));
        //Unbalanced list check
        Assert.False(mres.Wait(1000), "someone sent in balanced lists!!!");

        async Task HandleA((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            sourceArrived.TrySetResult(e);
            await readyPlayerOne.Task;
            readyPlayerOne = new();
        }

        async Task HandleB((int i, int x) e)
        {
            if (e.x % 2 != 0) return;
            var sourceEvent = await sourceArrived.Task;
            sourceArrived = new TaskCompletionSource<(int i, int x)>();
            readyPlayerOne.TrySetResult();
            Assert.Equal(sourceEvent.i, e.i);
            Assert.Equal(sourceEvent.x, e.x);
        }

        return;
        async Task Subscribe(IEnumerator<(int eventNumber, int eventData)> enumerator, 
                             Func<(int i, int x), Task> handler, 
                             int delay,
                             Action? onLive = null)
        {
            while (enumerator.MoveNext())
            {
                await Task.Delay(delay);
                await handler(enumerator.Current);
            }
            onLive?.Invoke();
        }
    }
}