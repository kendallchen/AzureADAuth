using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Data.Model;

namespace Data
{
    public class AzureADAuthDbContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }

        public AzureADAuthDbContext(DbContextOptions<AzureADAuthDbContext> options)
            : base(options)
        {
        }
    }
}
