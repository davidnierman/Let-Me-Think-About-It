namespace DidYouMeanCars
{
    public class InventoryServiceTests
    {
        private readonly Inventory _inventory;
        public InventoryServiceTests()
        {
            _inventory = new Inventory();
        }
        //sku?
        //location?
        [Fact]
        public void CanAddItemToEmptyInventory()
        {
            //Given a slot
            //TODO: make sure we don't have duplicate locations?
            /* Area < 4
             * Aisle    50  < 255
             * Shelfset 50  < 255
             * Shelf    8   < 255
             * bin      5   < 255
             * int = 4 sets of 255
             */
            _inventory.Add(
                new StorageLocation(new Area("A"), 
                new Aisle(1), 
                new Bay("A"),
                new Shelf(1), 
                new Bin("A")));
            //when
            var sku = new Sku(new Manufacturer("Chocolaticas"), new Model("Princess Women's Mary Jane Flat"),
                new Color("Pink"), new Size(39));
            var result = _inventory.AddItemToInventory(sku);
            //then
            Assert.True(result);
        }


        [Fact]
        public void CannotAddItemToInventoryWithNoAvailableLocations()
        {
            //Given nothing
            //when
            var sku = new Sku(new Manufacturer("Chocolaticas"), new Model("Princess Women's Mary Jane Flat"),
                new Color("Pink"), new Size(39));
            var result = _inventory.AddItemToInventory(sku);
            //then
            Assert.False(result);
        }

        [Fact]
        public void CannotAddItemToFullInventory()
        {
            _inventory.Add(
                new StorageLocation(new Area("A"), 
                    new Aisle(1), 
                    new Bay("A"),
                    new Shelf(1), 
                    new Bin("A")));
            var sku = new Sku(new Manufacturer("Chocolaticas"), new Model("Princess Women's Mary Jane Flat"),
                new Color("Pink"), new Size(39));
            _inventory.AddItemToInventory(sku);
            var result = _inventory.AddItemToInventory(sku);
            Assert.False(result);
        }
    }

    class Inventory
    {
        
        private readonly Stack<StorageLocation> _availableLocations = new ();

        public bool AddItemToInventory(Sku sku)
        {
            if (!_availableLocations.TryPop(out _)) return false;
            return true;
        }

        public void Add(StorageLocation storageLocation)
        {
            _availableLocations.Push(storageLocation);
        }
    }
}
