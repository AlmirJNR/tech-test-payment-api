namespace ECommerce.Data.Models
{
    public partial class PurchaseStatus
    {
        public PurchaseStatus()
        {
            Purchases = new HashSet<Purchase>();
        }

        public short Id { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
