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
        public static readonly DbContextOptions<LinksContext> options
            = new DbContextOptionsBuilder<LinksContext>()
            .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;
        public static IConfiguration configuration = null;

        private static readonly IList<int> oldIDs = new List<int> { 1643788, 4881095 };
        private static readonly IList<string> newIDs = new List<string> { "0015-7983-2945", "0015-7899-6240" };

        [TestInitialize]
        public void SetupDb()
        {
            System.Diagnostics.Debug.WriteLine("Here's the line before the previous statement");

            using (var context = new LinksContext(options))
            {
                System.Diagnostics.Debug.WriteLine("Here's the line before the break");
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
                    OldID = oldIDs[1],
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
                    NewID = newIDs[1],
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
            using (var context = new LinksContext(options))
            {
                context.CustomerLink.RemoveRange(context.CustomerLink);
                context.SaveChanges();
            };
        }

        [TestMethod]
        public async Task TestPutLinkNoContentResult()
        {
            using (var linkContext = new LinksContext(options))
            {
                string newID = newIDs[1];
                CustomerLink customer1 = linkContext.CustomerLink.Where(x => x.NewID == newIDs[1]).Single();
                customer1.OldID = oldIDs[1];

                CustomerLinksController linksController = new CustomerLinksController(linkContext);
                IActionResult result = await linksController.PutCustomerLink(customer1.ID, customer1) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

        public static void Main(string args)
        {
            UnitTestfboAPI tester = new UnitTestfboAPI();
            tester.SetupDb();

        }
    }
}
