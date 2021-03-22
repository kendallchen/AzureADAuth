using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Data;
using Data.Model;
using Core;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public ICustomerData CustomerData { get; }

        public CustomerController(ICustomerData customerData)
        {
            CustomerData = customerData;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] PageSortParam pageSortParam)
        {
            PageList<Customer> list = await this.CustomerData.Get(pageSortParam);
            //return result metadata in the header
            Response.Headers.Add("X-PageSortResult", JsonSerializer.Serialize(list.Result));
            return Ok(list);
        }

        // GET: api/Customer/5
        [HttpGet("{customerId}")]
        public async Task<ActionResult<Customer>> GetCustomer(int customerId)
        {
            return Ok(await this.CustomerData.GetCustomerById(customerId));
        }

        // PUT: api/Customer/5
        [HttpPut("{customerId}")]
        public async Task<ActionResult<Customer>> PutCustomer(int customerId, Customer customer)
        {
            return Ok(await this.CustomerData.Update(customerId, customer));
        }

        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            return Ok(await this.CustomerData.Add(customer));
        }

        // DELETE: api/Customer/5
        [HttpDelete("{customerId}")]
        public async Task<ActionResult<int>> DeleteCustomer(int customerId)
        {
            return Ok(await this.CustomerData.Delete(customerId));
        }
    }
}
