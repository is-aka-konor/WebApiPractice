using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
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

        public async Task<Customer> GetCustomerByExternalId(Guid externalId, CancellationToken cancellationToken)
        {
            return await this._appDbContext.Customers.Include(c => c.ContactDetails)
                                            .FirstOrDefaultAsync(x => x.CustomerExternalId.Equals(externalId), cancellationToken)
                                            .ConfigureAwait(false);
        }

        public async Task<Customer> SaveCustomer(Customer customer, CancellationToken cancellationToken)
        {
            customer.RowVersion = RowVersionGenerator.GetVersion();
            await this._appDbContext.Customers.AddAsync(customer).ConfigureAwait(false);
            await this._appDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return customer;
        }

        public async Task<Customer> UpdateCustomer(Customer customer, CancellationToken cancellationToken)
        {
            customer.RowVersion = RowVersionGenerator.GetVersion();
            this._appDbContext.Customers.Update(customer);
            await this._appDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return customer;
        }
    }
}
