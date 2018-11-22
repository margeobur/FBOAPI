using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fboAPI.Models
{
    public class LinksSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FboAPIContext(
                serviceProvider.GetRequiredService<DbContextOptions<FboAPIContext>>()))
            {
                if (context.CustomerLink.Count() > 0)
                {
                    return;
                }

                context.CustomerLink.AddRange(
                    new CustomerLink
                    {
                        OldData = {
                            Id = 1643788,
                            Username = "emusk10",
                            FirstName = "Elon",
                            Surname = "Musk",
                            Address = "Los Angeles",
                        },
                        NewData = 
                        {
                            Id = "0015-7983-2945",
                            Username = "elon_musk",
                            GivenNames = {"Elon", "Reeve", "Musk" },
                            Email = "emusk@gmail.com"
                        }
                    }
                );

            context.SaveChanges();
        }
    }
}
}
