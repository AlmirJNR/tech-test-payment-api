namespace ECommerce.Data.Models
{
    public partial class PurchaseProduct
    {
        public Guid PurchaseId { get; set; }
        public Guid ProductId { get; set; }
        public short? ProductAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual Purchase Purchase { get; set; } = null!;
    }
}
