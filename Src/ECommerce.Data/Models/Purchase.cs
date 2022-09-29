namespace ECommerce.Data.Models
{
    public partial class Purchase
    {
        public Purchase()
        {
            PurchaseProducts = new HashSet<PurchaseProduct>();
        }

        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public short? PurchaseStatusId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual PurchaseStatus? PurchaseStatus { get; set; }
        public virtual Seller Seller { get; set; } = null!;
        public virtual ICollection<PurchaseProduct> PurchaseProducts { get; set; }
    }
}
