namespace ECommerce.Data.Models
{
    public partial class Product
    {
        public Product()
        {
            PurchaseProducts = new HashSet<PurchaseProduct>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public short? Amount { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<PurchaseProduct> PurchaseProducts { get; set; }
    }
}
