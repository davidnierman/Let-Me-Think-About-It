namespace DidYouMeanCars
{
    public class InventoryTake2Tests
    {
        [Fact]
        public void HowDoWeKnowWhatProductItIs()
        {

        }

        [Fact]
        public void WhatIsTheFinalDestinationForStorage()
        {

        }

        [Fact]
        public void WhereAreWeMovingItRightNow()
        {

        }

        [Fact]
        public void WhereAreWeMovingItToNext()
        {

        }

        //Identify the product based on code
        // or unknown product
        //  what to do with exceptions here?
        //  how do we resolve knowing what it is?
        //Identify probable long-term storage location
        // put on correct intermediary location
    }

    public class ProductCatalogReadModel
    {
        public Sku? FindProduct(string barcode)
        {
            return null;
        }
    }

    public class EmptyStorageLocationReadModel
    {
        public StorageLocation? FindLocation(Sku sku)
        {
            return null;
        }
    }

    public class TransitManager
    {
        public StorageLocation AllocateTransitLocation()
        {
            return new StorageLocation();
        }
    }

    //public class TransitLocation
    //{
    //    private readonly TransitManager _manager;

    //    public TransitLocation(TransitManager manager)
    //    {
    //        _manager = manager;
    //    }

    //    public bool Add(Sku sku)
    //    {
    //    }
    //}

}
