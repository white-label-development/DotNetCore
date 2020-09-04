namespace InvoiceManagementApp.Application.Products.ViewModels
{
    public class AllProductVm
    {
        // a subset of domain product for list view

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal BuyingPrice { get; set; }
    }
}
