namespace DidYouMeanCars
{
    public class StorageConfigurationTests
    {
        /*
         * Divide areas into uniform types of aisle/shelfset/shelf/bin
         *  prevents having to specify all locations
         *  special casing
         */

        [Fact]
        public void CanCreateArea()
        {
            var sc = new StorageConfiguration();
            sc.DefineArea(
                area: "WC",
                aisles: 40,
                shelfSetsPerAisle: 10,
                shelvesPerShelfSet: 10,
                binsPerShelf: 5
            );

            Assert.Equal(
                new Event[]
                {
                    new AreaDefined(
                        Area: "WC",
                        Aisles: 40,
                        ShelfSetsPerAisle: 10,
                        ShelvesPerShelfSet: 10,
                        BinsPerShelf: 5
                    )
                }, 
                sc.ConsumePendingEvents());
        }

        [Fact]
        public void CannotDefineAreaMoreThanOnce()
        {
            var sc = new StorageConfiguration();
            Given(sc, new AreaDefined(
                Area: "WC",
                Aisles: 40,
                ShelfSetsPerAisle: 10,
                ShelvesPerShelfSet: 10,
                BinsPerShelf: 5
            ));
            Assert.Throws<InvalidOperationException>(()=>
            sc.DefineArea(
                area: "WC",//entity id
                aisles: 10,
                shelfSetsPerAisle: 10,
                shelvesPerShelfSet: 10,
                binsPerShelf: 5
            ));
        }

        [Fact]
        public void DefiningAnAreaIsIdempotent()
        {
            var sc = new StorageConfiguration();
            Given(sc, new AreaDefined(
                Area: "WC",
                Aisles: 40,
                ShelfSetsPerAisle: 10,
                ShelvesPerShelfSet: 10,
                BinsPerShelf: 5
            ));

            sc.DefineArea(
                area: "WC", //entity id
                aisles: 40,
                shelfSetsPerAisle: 10,
                shelvesPerShelfSet: 10,
                binsPerShelf: 5
            );
            Assert.Equal([], sc.ConsumePendingEvents());
        }

        private const string ValidAreaName = "ABC";
        private const int ValidAisleCount = 1;
        private const int ValidShelfSetsPerAisle = 1;
        private const int ValidShelvesPerShelfSet = 1;
        private const int ValidBinsPerShelf = 1;
        [Theory]
        [InlineData(nameof(area), typeof(ArgumentNullException),null!, ValidAisleCount, ValidShelfSetsPerAisle,ValidShelvesPerShelfSet, ValidBinsPerShelf)]
        [InlineData(nameof(aisles), typeof(ArgumentOutOfRangeException),ValidAreaName,-1, ValidShelfSetsPerAisle,ValidShelvesPerShelfSet, ValidBinsPerShelf)]
        [InlineData(nameof(aisles), typeof(ArgumentOutOfRangeException),ValidAreaName,0, ValidShelfSetsPerAisle,ValidShelvesPerShelfSet, ValidBinsPerShelf)]
        [InlineData(nameof(shelfSetsPerAisle), typeof(ArgumentOutOfRangeException),ValidAreaName,ValidAisleCount, 0,ValidShelvesPerShelfSet, ValidBinsPerShelf)]
        [InlineData(nameof(shelfSetsPerAisle), typeof(ArgumentOutOfRangeException),ValidAreaName,ValidAisleCount, -1,ValidShelvesPerShelfSet, ValidBinsPerShelf)]
        public void ValidateParameters(string paramName, Type exceptionType, string area, int aisles,int shelfSetsPerAisle, int shelvesPerShelfSet, int binsPerShelf) 
        {
            var sc = new StorageConfiguration();

            var ex = Assert.ThrowsAny<ArgumentException>(() =>
                sc.DefineArea(area, aisles, shelfSetsPerAisle, shelvesPerShelfSet, binsPerShelf));
            Assert.IsType(exceptionType, ex);
            Assert.Equal(paramName, ex.ParamName);
        }

        public static IEnumerable<object[]> AlternativeParameterValidationTests()
        {
            yield return ["area", typeof(ArgumentNullException), null!, 1];
            yield return ["aisles", typeof(ArgumentOutOfRangeException), "ABC", -1];
        }

        [Theory]
        [MemberData(nameof(AlternativeParameterValidationTests))]
        public void ValidateParameters2(string paramName, Type exceptionType, string area, int aisles) 
        {
            var sc = new StorageConfiguration();

            var ex = Assert.ThrowsAny<ArgumentException>(() =>
                sc.DefineArea(area, aisles, 1, 1, 1));
            Assert.IsType(exceptionType, ex);
            Assert.Equal(paramName, ex.ParamName);
        }

        [Fact]
        public void LocationFormattingAndParsing()
        {
            StorageLocation storageLocation = "WC-20-C-10-D";
            Assert.Equal("WC-20-C-10-D", storageLocation.ToString());
            Assert.Equal((Area)"WC", storageLocation.Area);

            Assert.Equal((Bay)"C", storageLocation.Bay);
            Assert.Equal((Bin)"D", storageLocation.Bin);
        }

        private static void Given(StorageConfiguration sc, params Event[] events)
        {
            foreach (var e in events)
            {
                sc.Apply(e);
            }
        }
    }
}
