namespace ECommerce.Data.Models
{
    public partial class Seller
    {
        public Seller()
        {
            Purchases = new HashSet<Purchase>();
        }

        public Guid Id { get; set; }
        public string Cpf { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Telephone { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
