namespace Subscription.Tests
{
    public class UnitTest1
    {
        private object?[] _queue = new object[32];

        [Fact]
        public async Task NotifyOnWrite()
        {
            var task = Read(0);
            Assert.False(task.IsCompleted);
            await Write(0);
            Assert.True(task.IsCompleted);
        }

        public Task<object> Read(int position)
        {
            var notification = new TaskCompletionSource<object>();
            var read = Interlocked.CompareExchange(ref _queue[position], notification, null);
            if (read == null)
            {
                return notification.Task;
            }

            return Task.FromResult(read);

        }

        public Task Write(int value)
        {
            var result = Interlocked.CompareExchange(ref _queue[0], value, null);
            if (result == null)
            {
                return Task.CompletedTask;
            }

            result = Interlocked.CompareExchange(ref _queue[0], value, result);
            if (result is TaskCompletionSource<object> tcs)
            {
                tcs.TrySetResult(value);
            }
            return Task.CompletedTask;
        }
    }
}