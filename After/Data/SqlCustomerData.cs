using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Core;

namespace Data
{
    public class SqlCustomerData : ICustomerData
    {
        public AzureADAuthDbContext DbContext { get; }

        public SqlCustomerData(AzureADAuthDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<Customer> Add(Customer customer)
        {
            DbContext.Add(customer);
            await DbContext.SaveChangesAsync();
            return customer;
        }

        public async Task<int> Delete(int customerId)
        {
            Customer c = await this.GetCustomerById(customerId);
            if (c != null)
            {
                this.DbContext.Remove(c);
                await DbContext.SaveChangesAsync();
                return customerId;
            }
            return -1;
        }

        public async Task<PageList<Customer>> Get(PageSortParam pageSortParam)
        {
            PageList<Customer> list = new PageList<Customer>(pageSortParam);
            
            await list.GetData(DbContext.Customer);
            return list;
        }

        public async Task<Customer> GetCustomerById(int customerId)
        {
            Customer c = await this.DbContext.Customer.FindAsync(customerId);
            if (c != null)
                return c;
            return null;
        }

        public async Task<Customer> Update(int customerId, Customer customer)
        {
            Customer c = await GetCustomerById(customerId);
            if (c != null)
            {
                c.FirstName = customer.FirstName;
                c.LastName = customer.LastName;
                await DbContext.SaveChangesAsync();
                return c;
            }
            return null;
        }
    }
}
