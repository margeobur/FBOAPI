using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace fboAPI.Models
{
    public class LinksContext : DbContext
    {
        public LinksContext (DbContextOptions<LinksContext> options)
            : base(options)
        {
        }

        public DbSet<fboAPI.Models.CustomerLink> CustomerLink { get; set; }
    }
}
