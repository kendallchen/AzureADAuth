using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace Data
{
    public interface ICustomerData
    {
        Task<PageList<Customer>> Get(PageSortParam pageSort);
        Task<Customer> GetCustomerById(int customerId);
        Task<Customer> Update(int customerId, Customer customer);
        Task<Customer> Add(Customer customer);
        Task<int> Delete(int customerId);
    }
}
