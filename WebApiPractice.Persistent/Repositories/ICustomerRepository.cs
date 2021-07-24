using System;
using System.Threading.Tasks;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetCustomerByExternalId(Guid externalId);
        Task<Customer> SaveCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
    }
}