using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace fboAPI.Data
{
    public class NewCustomersContext : DbContext
    {
        public NewCustomersContext(DbContextOptions<NewCustomersContext> options)
            : base(options)
        {
        }

        public DbSet<fboAPI.Models.NewCustomer> NewCustomer { get; set; }
    }
}
