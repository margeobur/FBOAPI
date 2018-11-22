using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace fboAPI.Models
{
    public class FboAPIContext : DbContext
    {
        public FboAPIContext (DbContextOptions<FboAPIContext> options)
            : base(options)
        {
        }

        public DbSet<fboAPI.Models.CustomerLink> CustomerLink { get; set; }
    }
}
