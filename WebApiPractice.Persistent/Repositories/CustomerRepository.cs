using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _appDbContext;

        public CustomerRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task<Customer> GetCustomerByExternalId(Guid externalId)
        {
            return await this._appDbContext.Customers.Include(c => c.ContactDetails).FirstOrDefaultAsync(x => x.CustomerExternalId.Equals(externalId)).ConfigureAwait(false);
        }

        public async Task<Customer> SaveCustomer(Customer customer)
        {
            customer.RowVersion = RowVersionGenerator.GetVersion();
            await this._appDbContext.Customers.AddAsync(customer).ConfigureAwait(false);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            customer.RowVersion = RowVersionGenerator.GetVersion();
            this._appDbContext.Customers.Update(customer);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return customer;
        }
    }
}
