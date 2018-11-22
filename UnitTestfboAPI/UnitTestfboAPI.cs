using fboAPI.Controllers;
using fboAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;


namespace UnitTestfboAPI
{
    [TestClass]
    public class UnitTestfboAPI
    {
        public static readonly DbContextOptions<FboAPIContext> options
            = new DbContextOptionsBuilder<FboAPIContext>()
            .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;
        public static IConfiguration configuration = null;

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new FboAPIContext(options))
            {
                CustomerLink customer1 = new CustomerLink()
                {
                    OldData = {
                            Id = 1643788,
                            Username = "emusk10",
                            FirstName = "Elon",
                            Surname = "Musk",
                            Address = "Los Angeles",
                    },
                    NewData = {
                            Id = "0015-7983-2945",
                            Username = "elon_musk",
                            GivenNames = {"Elon", "Reeve", "Musk" },
                            Email = "emusk@gmail.com"
                    }
                };

                CustomerLink customer2 = new CustomerLink()
                {
                    OldData = {
                            Id = 4881095,
                            Username = "jwhales29",
                            FirstName = "Jimmy",
                            Surname = "Whales",
                            Address = "San Francisco"
                    }
                };

                CustomerLink customer3 = new CustomerLink()
                {
                    NewData = {
                            Id = "0015-7899-6240",
                            Username = "jwhales29",
                            GivenNames = { "Jimmy", "Whales" },
                            Email = "jwhale@hotmail.com"
                    }
                };


                context.CustomerLink.Add(customer1);
                context.CustomerLink.Add(customer2);
                context.CustomerLink.Add(customer3);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new FboAPIContext(options))
            {
                context.CustomerLink.RemoveRange(context.CustomerLink);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestMethod1()
        {

        }
    }
}
