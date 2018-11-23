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
        public async Task<IActionResult> GetCombinedLink([FromRoute] Guid id)
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

            return Ok(customerLink);
        }

        // PUT: api/CustomerLinks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCombinedLink([FromRoute] Guid id, [FromBody] CombinedLink combinedLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != combinedLink.Link.ID)
            {
                return BadRequest();
            }

            _linkContext.Entry(combinedLink).State = EntityState.Modified;

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

            _linkContext.CustomerLink.Add(combinedLink.Link);
            await _linkContext.SaveChangesAsync();

            return CreatedAtAction("GetCustomerLink", new { id = combinedLink.Link.ID }, combinedLink);
        }

        // DELETE: api/CustomerLinks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCombinedLink([FromRoute] Guid id)
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

            _linkContext.CustomerLink.Remove(customerLink);
            await _linkContext.SaveChangesAsync();

            return Ok(customerLink);
        }

        private bool CustomerLinkExists(Guid id)
        {
            return _linkContext.CustomerLink.Any(e => e.ID == id);
        }

        private CombinedLink GetFullLinkFromContexts(CustomerLink bareCustomerLink)
        {
            NewCustomer newCustomerData = null;
            if (bareCustomerLink.NewID != null)
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