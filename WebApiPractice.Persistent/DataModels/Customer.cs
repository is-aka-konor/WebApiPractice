using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace WebApiPractice.Persistent.DataModels
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public Guid CustomerExternalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<ContactDetails> ContactDetails { get; set; } = null!;
        public List<Note> Notes { get; set; } = null!;
    }

    public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(e => e.CustomerId);
            builder.Property(e => e.CustomerId).HasColumnName("Id");
            builder.Property(e => e.CustomerExternalId).HasColumnName("ExternalId");
            builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            builder.Property(e => e.FirstName).HasMaxLength(30);
            builder.Property(e => e.LastName).HasMaxLength(70);
            builder.Property(e => e.Status).HasMaxLength(20);
        }
    }
}
