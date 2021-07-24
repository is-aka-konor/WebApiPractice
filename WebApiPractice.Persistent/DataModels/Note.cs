using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
namespace WebApiPractice.Persistent.DataModels
{
    public class Note
    {
        public int NoteId { get; set; }
        public Guid NoteExternalId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public string RowVersion { get; set; } = RowVersionGenerator.GetVersion();
        public Customer Customer { get; set; } = null!;
    }

    public class NoteEntityConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.ToTable("Notes");
            builder.HasKey(e => e.NoteId);
            builder.Property(e => e.NoteId).HasColumnName("Id");
            builder.Property(e => e.NoteExternalId).HasColumnName("ExternalId");
            builder.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            builder.Property(e => e.NoteText).HasMaxLength(1000);
        }
    }
}
