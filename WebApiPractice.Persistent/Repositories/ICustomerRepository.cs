using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByExternalId(Guid externalId, CancellationToken cancellationToken);
        Task<Customer> SaveCustomer(Customer customer, CancellationToken cancellationToken);
        Task<Customer> UpdateCustomer(Customer customer, CancellationToken cancellationToken);
    }
}