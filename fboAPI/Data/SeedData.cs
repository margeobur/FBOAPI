using fboAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fboAPI.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new OldCustomersContext(
                serviceProvider.GetRequiredService<DbContextOptions<OldCustomersContext>>()))
            {
                if(context.OldCustomer.Count() == 0)
                {
                    context.OldCustomer.AddRange(
                       new OldCustomer
                       {
                           Id = 1643788,
                           Username = "emusk10",
                           FirstName = "Elon",
                           Surname = "Musk",
                           Address = "Los Angeles",
                       }
                   );

                    context.SaveChanges();
                }
            }

            using (var context = new NewCustomersContext(
                serviceProvider.GetRequiredService<DbContextOptions<NewCustomersContext>>()))
            {
                if (context.NewCustomer.Count() == 0)
                {
                    context.NewCustomer.AddRange(
                    new NewCustomer
                    {
                        Id = "0015-7983-2945",
                        Username = "elon_musk",
                        GivenNames = { "Elon", "Reeve", "Musk" },
                        Email = "emusk@gmail.com"
                    }
                );

                    context.SaveChanges();
                }
            }

            using (var context = new LinksContext(
                serviceProvider.GetRequiredService<DbContextOptions<LinksContext>>()))
            {
                if(context.CustomerLink.Count() == 0)
                {
                    context.CustomerLink.AddRange(
                    new CustomerLink
                    {
                        ID = Guid.NewGuid().ToString(),
                        OldID = 1643788,
                        NewID = "0015-7983-2945"
                    }
                );

                    context.SaveChanges();
                }
            }
        }
    }
}
