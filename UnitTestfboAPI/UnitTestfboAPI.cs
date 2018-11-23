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

        [TestMethod]
        public void TestRegularGetCorrectResult()
        {
            using (var linksContext = new LinksContext(linksOptions))
            {
                CustomerLink customerLink0 = linksContext.CustomerLink.Where(x => x.NewID == newIDs[0]).Single();
                CustomerLink customerLink1a = linksContext.CustomerLink.Where(x => x.OldID == oldIDs[1]).Single();
                CustomerLink customerLink1b = linksContext.CustomerLink.Where(x => x.NewID == newIDs[1]).Single();

                CombinedLink customerData0 = GetFullLinkFromContexts(customerLink0);
                CombinedLink customerData1a = GetFullLinkFromContexts(customerLink1a);
                CombinedLink customerData1b = GetFullLinkFromContexts(customerLink1b);

                NewCustomersContext newContext = new NewCustomersContext(newCOptions);
                OldCustomersContext oldContext = new OldCustomersContext(oldCOptions);
                CustomerLinksController linksController = new CustomerLinksController(linksContext, newContext, oldContext);
                IEnumerable<CombinedLink> customers = linksController.GetCombinedLink();

                newContext.Dispose();
                oldContext.Dispose();

                bool[] foundCustomer = new bool[3];
                foundCustomer[0] = false;
                foundCustomer[1] = false;
                foundCustomer[2] = false;

                System.Console.WriteLine("Looking for customers...");
                System.Diagnostics.Debug.WriteLine("Looking for customers...");

                foreach (CombinedLink customer in customers)
                {
                    System.Diagnostics.Debug.WriteLine("Customer found:");
                    System.Console.WriteLine("Customer found");

                    if (customer.Equals(customerData0))
                    {
                        foundCustomer[0] = true;
                    }
                    else if(customer.Equals(customerData1a))
                    {
                        foundCustomer[1] = true;
                    }
                    else if(customer.Equals(customerData1b))
                    {
                        foundCustomer[2] = true;
                    }
                }

                if(!foundCustomer[0])
                    Assert.Fail("There was no data for Elon Musk");

                if(!foundCustomer[1])
                    Assert.Fail("There was no old data for Jimmy Wales");

                if(!foundCustomer[2])
                    Assert.Fail("There was no new data for Jimmy Wales");

                if (customers.Count() != 3)
                    Assert.Fail("There were more than 3 customer links");
            };
        }

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
                if (bareCustomerLink.NewID != null)
                {
                    IQueryable<NewCustomer> filteredContext = 
                        newContext.NewCustomer.Where(x => x.Id == bareCustomerLink.NewID);
                    if (filteredContext.Count() == 1)
                        newCustomerData = filteredContext.Single();
                }
            }

            OldCustomer oldCustomerData = null;
            using (OldCustomersContext oldContext = new OldCustomersContext(oldCOptions))
            {
                if (bareCustomerLink.OldID != null)
                {
                    IQueryable<OldCustomer> filteredContext = 
                        oldContext.OldCustomer.Where(x => x.Id == bareCustomerLink.OldID);
                    if(filteredContext.Count() == 1)
                        oldCustomerData = filteredContext.Single();
                }
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
