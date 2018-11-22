using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace fboAPI.Data
{
    public class OldCustomersContext : DbContext
    {
        public OldCustomersContext(DbContextOptions<OldCustomersContext> options)
            : base(options)
        {
        }

        public DbSet<fboAPI.Models.OldCustomer> OldCustomer { get; set; }
    }
}
