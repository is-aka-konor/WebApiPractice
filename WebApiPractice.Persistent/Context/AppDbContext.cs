using Microsoft.EntityFrameworkCore;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Context
{
    public class AppDbContext : DbContext
    {
        #region Constructors
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected AppDbContext(DbContextOptions options) : base(options) { }
        #endregion
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Note> Notes => Set<Note>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CustomerEntityConfiguration());
            builder.ApplyConfiguration(new NoteEntityConfiguration());
            builder.ApplyConfiguration(new ContactDetailsEntityConfiguration());
        }
    }
}
