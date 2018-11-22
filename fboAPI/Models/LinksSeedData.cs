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
                // Look for any movies.
                if (context.CustomerLink.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.CustomerLink.AddRange(
                    new CustomerLink
                    {
                        oldData = {
                            id = 1643788,
                            username = "emusk10",
                            firstName = "Elon",
                            surname = "Musk",
                            address = "Los Angeles",
                        },
                        newData = 
                        {
                            id = "0015-7983-2945",
                            username = "elon_musk",
                            GivenNames = {"Elon", "Reeve", "Musk" },
                            email = "emusk@gmail.com"
                        }
                    }
                );

            //{
            //    Title = "Is Mayo an Instrument?",
            //        Url = "https://i.kym-cdn.com/photos/images/original/001/371/723/be6.jpg",
            //        Tags = "spongebob",
            //        Uploaded = "07-10-18 4:20T18:25:43.511Z",
            //        Width = "768",
            //        Height = "432"
            //}

            context.SaveChanges();
        }
    }
}
}
