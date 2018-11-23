using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fboAPI.Data;
using fboAPI.Models;

namespace fboAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerLinksController : ControllerBase
    {
        private readonly LinksContext _linkContext;
        private readonly NewCustomersContext _newContext;
        private readonly OldCustomersContext _oldContext;

        public CustomerLinksController(LinksContext context, NewCustomersContext newContext, OldCustomersContext oldContext)
        {
            _linkContext = context;
            _newContext = newContext;
            _oldContext = oldContext;
        }

        // GET: api/CustomerLinks
        [HttpGet]
        public IEnumerable<CombinedLink> GetCombinedLink()
        {
            List<CombinedLink> allCustomerData = new List<CombinedLink>();

            int numberTaken = 0;
            foreach (CustomerLink link in _linkContext.CustomerLink)
            {
                allCustomerData.Add(GetFullLinkFromContexts(link));
                if (numberTaken++ == 100)
                    break;
            }
            //return _context.CustomerLink;
            return allCustomerData;
        }

        // GET: api/CustomerLinks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCombinedLink([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerLink = await _linkContext.CustomerLink.FindAsync(id);

            if (customerLink == null)
            {
                return NotFound();
            }

            CombinedLink fullData = GetFullLinkFromContexts(customerLink);

            return Ok(fullData);
        }

        // PUT: api/CustomerLinks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombinedLink([FromRoute] string id, [FromBody] CombinedLink combinedLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != combinedLink.Link.ID)
            {
                return BadRequest();
            }

            _linkContext.Entry(combinedLink.Link).State = EntityState.Modified;

            if (combinedLink.Link.NewID != "" && combinedLink.Link.OldID != -1)
            {
                // We want to remove the other CustomerLink that was combined.
                // First search for a link with the same ID for the customer's new data
                IEnumerable<CustomerLink> filteredContext =
                    _linkContext.CustomerLink.Where(x => (x.NewID == combinedLink.Link.NewID
                                                         || x.OldID == combinedLink.Link.OldID));
                if(filteredContext.Count() > 1)
                {
                    List<CustomerLink> toRemove = new List<CustomerLink>();
                    foreach (CustomerLink linkedData in filteredContext)
                        if(linkedData.ID != combinedLink.Link.ID)
                        {
                            toRemove.Add(linkedData);
                        }

                    foreach (CustomerLink linkToRemove in toRemove)
                        _linkContext.CustomerLink.Remove(linkToRemove);
                }
            }

            try
            {
                await _linkContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerLinkExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CustomerLinks
        [HttpPost]
        public async Task<IActionResult> PostCombinedLink([FromBody] CombinedLink combinedLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(combinedLink.NewC != null)
            {
                combinedLink.NewC.Id = Guid.NewGuid().ToString();
                combinedLink.Link.NewID = combinedLink.NewC.Id;
                _newContext.NewCustomer.Add(combinedLink.NewC);
                await _newContext.SaveChangesAsync();
            }

            if(combinedLink.OldC != null)
            {
                combinedLink.OldC.Id = _oldContext.OldCustomer.Count() + 1;
                combinedLink.Link.OldID = combinedLink.OldC.Id;
                _oldContext.OldCustomer.Add(combinedLink.OldC);
                await _oldContext.SaveChangesAsync();
            }

            System.Diagnostics.Debug.WriteLine("Adding combinedLink with " +
                combinedLink.Link.ID == null ? "null" : combinedLink.Link.ID.ToString());
            System.Console.WriteLine("Adding combinedLink with " +
                combinedLink.Link.ID == null ? "null" : combinedLink.Link.ID.ToString());

            combinedLink.Link.ID = Guid.NewGuid().ToString();
            _linkContext.CustomerLink.Add(combinedLink.Link);
            await _linkContext.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine("combinedLink ID is now " +
                combinedLink.Link.ID == null ? "null" : combinedLink.Link.ID.ToString());
            System.Console.WriteLine("combinedLink ID is now " +
               combinedLink.Link.ID == null ? "null" : combinedLink.Link.ID.ToString());

            return Ok($"Customer with id {combinedLink.Link.ID} has successfully been mde");
        }

        // DELETE: api/CustomerLinks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombinedLink([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerLink = await _linkContext.CustomerLink.FindAsync(id);
            if (customerLink == null)
            {
                return NotFound();
            }

            CombinedLink combinedLink = GetFullLinkFromContexts(customerLink);

            if (combinedLink.NewC != null)
            {
                _newContext.NewCustomer.Remove(combinedLink.NewC);
                await _newContext.SaveChangesAsync();
            }

            if (combinedLink.OldC != null)
            {
                _oldContext.OldCustomer.Remove(combinedLink.OldC);
                await _oldContext.SaveChangesAsync();
            }

            _linkContext.CustomerLink.Remove(customerLink);
            await _linkContext.SaveChangesAsync();

            return Ok(customerLink);
        }

        private bool CustomerLinkExists(string id)
        {
            return _linkContext.CustomerLink.Any(e => e.ID == id);
        }

        private CombinedLink GetFullLinkFromContexts(CustomerLink bareCustomerLink)
        {
            NewCustomer newCustomerData = null;
            if (bareCustomerLink.NewID != "")
            {
                IQueryable<NewCustomer> filteredContext =
                    _newContext.NewCustomer.Where(x => x.Id == bareCustomerLink.NewID);
                if (filteredContext.Count() == 1)
                    newCustomerData = filteredContext.Single();
            }

            OldCustomer oldCustomerData = null;
            if (bareCustomerLink.OldID != -1)
            {
                IQueryable<OldCustomer> filteredContext =
                    _oldContext.OldCustomer.Where(x => x.Id == bareCustomerLink.OldID);
                if (filteredContext.Count() == 1)
                    oldCustomerData = filteredContext.Single();
            }

            System.Diagnostics.Debug.WriteLine("Id: " /*+ oldCustomerData.Id*/);

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