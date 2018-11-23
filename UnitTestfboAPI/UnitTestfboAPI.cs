using fboAPI.Controllers;
using fboAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using fboAPI.Data;

namespace UnitTestfboAPI
{
    [TestClass]
    public class UnitTestfboAPI
    {
        public static readonly DbContextOptions<LinksContext> linksOptions
            = new DbContextOptionsBuilder<LinksContext>()
            .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;
        public static readonly DbContextOptions<NewCustomersContext> newCOptions
            = new DbContextOptionsBuilder<NewCustomersContext>()
            .UseInMemoryDatabase(databaseName: "testNewCDatabase")
            .Options;
        public static readonly DbContextOptions<OldCustomersContext> oldCOptions
            = new DbContextOptionsBuilder<OldCustomersContext>()
            .UseInMemoryDatabase(databaseName: "testOldCDatabase")
            .Options;
        public static IConfiguration configuration = null;

        private static readonly IList<int> oldIDs = new List<int> { 1643788, 4881095 };
        private static readonly IList<string> newIDs = new List<string> { "0015-7983-2945", "0015-7899-6240" };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new OldCustomersContext(oldCOptions))
            {
                OldCustomer customer0a = new OldCustomer()
                {
                    Id = oldIDs[0],
                    Username = "emusk10",
                    FirstName = "Elon",
                    Surname = "Musk",
                    Address = "Los Angeles",
                };

                OldCustomer customer1a = new OldCustomer()
                {
                    Id = oldIDs[1],
                    Username = "jwhales29",
                    FirstName = "Jimmy",
                    Surname = "Whales",
                    Address = "San Francisco"
                };

                context.OldCustomer.Add(customer0a);
                context.OldCustomer.Add(customer1a);
                context.SaveChanges();
            }

            using (var context = new NewCustomersContext(newCOptions))
            {
                NewCustomer customer0b = new NewCustomer()
                {
                    Id = newIDs[0],
                    Username = "elon_musk",
                    GivenNames = { "Elon", "Reeve", "Musk" },
                    Email = "emusk@gmail.com"
                };

                NewCustomer customer1b = new NewCustomer()
                {
                    Id = newIDs[1],
                    Username = "jwhales29",
                    GivenNames = { "Jimmy", "Whales" },
                    Email = "jwhale@hotmail.com"
                };

                context.NewCustomer.Add(customer0b);
                context.NewCustomer.Add(customer1b);
                context.SaveChanges();
            }

            using (var context = new LinksContext(linksOptions))
            {
                CustomerLink customer0 = new CustomerLink()
                {
                    OldID = oldIDs[0],
                    NewID = newIDs[0]
                };

                CustomerLink customer1a = new CustomerLink()
                {
                    OldID = oldIDs[1]
                };

                CustomerLink customer1b = new CustomerLink()
                {
                    NewID = newIDs[1]
                };

                context.CustomerLink.Add(customer0);
                context.CustomerLink.Add(customer1a);
                context.CustomerLink.Add(customer1b);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new LinksContext(linksOptions))
            {
                context.CustomerLink.RemoveRange(context.CustomerLink);
                context.SaveChanges();
            };

            using (var context = new OldCustomersContext(oldCOptions))
            {
                context.OldCustomer.RemoveRange(context.OldCustomer);
                context.SaveChanges();
            };

            using (var context = new NewCustomersContext(newCOptions))
            {
                context.NewCustomer.RemoveRange(context.NewCustomer);
                context.SaveChanges();
            };
        }

        //[TestMethod]
        //public async Task TestRegularGetCorrectResult()
        //{

        //}

        //[TestMethod]
        //public async Task TestRegularGetCorrectContent()
        //{

        //}

        //[TestMethod]
        //public async Task TestFilteredGet()
        //{

        //}

        //[TestMethod]
        //public async Task TestPostCorrectResult()
        //{

        //}

        //[TestMethod]
        //public async Task TestPostItemAdd()
        //{

        //}

        /* Perform a PUT linking two customers */
        private async Task<IActionResult> PutLinkCombination()
        {
            IActionResult result = null;
            using (var linksContext = new LinksContext(linksOptions))
            {
                CustomerLink customer1 = linksContext.CustomerLink.Where(x => x.NewID == newIDs[1]).Single();
                customer1.OldID = oldIDs[1];

                NewCustomersContext newContext = new NewCustomersContext(newCOptions);
                OldCustomersContext oldContext = new OldCustomersContext(oldCOptions);
                CustomerLinksController linksController = new CustomerLinksController(linksContext, newContext, oldContext);
                result = await linksController.PutCombinedLink(customer1.ID, GetFullLinkFromContexts(customer1)) as IActionResult;

                newContext.Dispose();
                oldContext.Dispose();
            }
            return result;
        }

        /* Test that the response is correct when a valid PUT is called */
        [TestMethod]
        public async Task TestPutLinkNoContentResult()
        {
            using (var linksContext = new LinksContext(linksOptions))
            {
                IActionResult result = await PutLinkCombination();

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }

        /* Test that the model is in fact updated when a valid PUT is called */
        [TestMethod]
        public async Task TestPutItemUpdate()
        {
            using (var linksContext = new LinksContext(linksOptions))
            {
                IActionResult result = await PutLinkCombination();

                // filter the customerLinks by the new ID. There should only be one
                // (if more than one an exception will be thrown and test will fail)
                CustomerLink customer = linksContext.CustomerLink.Where(x => x.NewID == newIDs[1]).Single();

                // filter the customerLinks by the old ID. There should only be one
                CustomerLink sameCustomer = linksContext.CustomerLink.Where(x => x.OldID == oldIDs[1]).Single();

                // They should be the same now (the PUT combines them)
                Assert.AreSame(customer, sameCustomer);
            }
        }

        /* This method packages the data from the different contexts into one combined object */
        private CombinedLink GetFullLinkFromContexts(CustomerLink bareCustomerLink)
        {
            NewCustomer newCustomerData = null;
            using (NewCustomersContext newContext = new NewCustomersContext(newCOptions))
            {
                newCustomerData = newContext.NewCustomer.Where(x => x.Id == bareCustomerLink.NewID).Single();
            }

            OldCustomer oldCustomerData = null;
            using (OldCustomersContext oldContext = new OldCustomersContext(oldCOptions))
            {
                oldCustomerData = oldContext.OldCustomer.Where(x => x.Id == bareCustomerLink.OldID).Single();
            }

            CombinedLink fullLink = new CombinedLink
            {
                Link = bareCustomerLink,
                OldC = oldCustomerData,
                NewC = newCustomerData
            };

            return fullLink;
        }
    }
}
