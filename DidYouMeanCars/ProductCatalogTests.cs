namespace DidYouMeanCars
{
    /*
     * DDD - Domain Driven Design
     * CQRS - Command Query Responsibility Segregation / Separation
     * ES - Event Sourcing
     *
     *
     *
     * online Shoe Shop inventory
     * SKU - how we identify the product
     *  serial no. ? - we won't do this
     * Products
     *  -Product Catalog
     * Locations
     *  -Where we can keep stuff
     * Inventory
     *  - what we have
     *  - where it is
     *
     * Target scenarios to support
     *  Receive delivery - ignore delivery reconciliation for now
     *   - Given a set of products arriving
     *    - Where to put
     *  Picking
     *   - Given a set of orders to pack
     *   - Where are the items to pick?
     *   - Generate a pick/pack list
     *  Relocation
     *   - Detect consolidation opportunities
     *   - Move stock between locations
     *   - Consolidate like stock?
     *  Stock Reconciliation
     *   - If we scan every item in every location
     *    - does each location contain what we thought it should?
     *    - what if it is damaged
     *    - Stock taking report
     *
     * Build a version aware load balancer
     *   - How do we version commands/events in a long-lived system
     * Dependency injection
     * HappyFeet 
     */

    public class ProductCatalogTests
    {
        [Fact]
        public void CanCreateProductCatalog()
        {
            var storeName = "HappyFeet";
            var pc = new ProductCatalog(storeName);
            Assert.Equal(
                new []{new ProductCatalogCreated(storeName)}, 
                pc.ConsumePendingEvents());
        }

        [Fact]
        public void CanAddSku()
        {
            // NIKE-MODEL1-WHITE-48 /48 eu/ 8 uk /9.5 us/ 27.5 jpn
            // Manufacturer, Model, Color, Size
            // string, string, Color, int
            
            var pc = InstanceFactory.Create<ProductCatalog>();
            Given(pc, new ProductCatalogCreated("HappyFeet"));
            Sku sku = "NIKE-MODEL1-White-48";
            pc.AddSku(sku);
            Assert.Equal(
                new Event[]{new SkuAdded(sku)}, 
                pc.ConsumePendingEvents());
        }

        [Fact]
        public void CanAddSkuIdempotently()
        {
            Sku sku = "NIKE-MODEL1-White-48";
            var pc = InstanceFactory.Create<ProductCatalog>();
            Given(pc, new ProductCatalogCreated("HappyFeet"),
                new SkuAdded(sku));
            pc.AddSku(sku);
            Assert.Equal(
                new Event[]{}, 
                pc.ConsumePendingEvents());
        }

        [Fact]
        public void CanAddMultipleSkus()
        {
            Sku sku1 = "NIKE-MODEL1-White-48";
            Sku sku2 = "NIKE-MODEL1-White-39";
            var pc = InstanceFactory.Create<ProductCatalog>();
            Given(pc, new ProductCatalogCreated("HappyFeet"),
                new SkuAdded(sku1));

            pc.AddSku(sku2);
            Assert.Equal(
                new Event[]{new SkuAdded(sku2)}, 
                pc.ConsumePendingEvents());
        }

        private static void Given(ProductCatalog pc, params Event[] events)
        {
            foreach (var e in events)
            {
                pc.Apply(e);
            }
        }
    }
}