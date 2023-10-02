namespace ReallyUsingTestFrameworks
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(1, 1, 2)]
        [InlineData(4, 8, 12)]
        [InlineData(3, 1, 4)]
        public void Test1(int a, int b, int result)
        {
            Assert.Equal(result, a + b);
        }

        [Fact]
        public void SomethingInappropriate()
        {
            Assert.Equal(43, 42);
        }

        [Fact]
        public async Task SomethingInappropriatelyAsync()
        {
            await Task.Delay(10);
            Assert.Equal(43, 42);
        }

        [Theory]
        [InlineData(1,1, 2)]
        [InlineData(2,1, 3)]
        [InlineData(3,1, 4)]
        public async void Test2(int a, int b, int result)
        {
            await Task.Delay(100);
            Assert.Equal(result, a + b);
        }
    }
}