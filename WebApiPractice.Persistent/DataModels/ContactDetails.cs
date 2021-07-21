using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApiPractice.Persistent.DataModels
{
    public class ContactDetails
    {
        public int ContactDetailsId { get; set; }
        public string ContactDetailsType { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public Customer Customer { get; set; } = null!;
    }

    public class ContactDetailsEntityConfiguration : IEntityTypeConfiguration<ContactDetails>
    {
        public void Configure(EntityTypeBuilder<ContactDetails> builder)
        {
            builder.ToTable("ContactDetails");
            builder.HasKey(e => e.ContactDetailsId);
            builder.Property(e => e.ContactDetailsId).HasColumnName("Id");
            builder.Property(e => e.ContactDetailsType).HasColumnName("Type");
            builder.Property(e => e.Details).HasMaxLength(100);
        }
    }
}
