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
        private readonly LinksContext _context;
        private readonly NewCustomersContext _newContext;
        private readonly OldCustomersContext _oldContext;

        public CustomerLinksController(LinksContext context, NewCustomersContext newContext, OldCustomersContext oldContext)
        {
            _context = context;
            _newContext = newContext;
            _oldContext = oldContext;
        }

        // GET: api/CustomerLinks
        [HttpGet]
        public IEnumerable<CombinedLink> GetCombinedLink()
        {
            //return _context.CustomerLink;
            return new List<CombinedLink>();
        }

        // GET: api/CustomerLinks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCombinedLink([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerLink = await _context.CustomerLink.FindAsync(id);

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

            _context.Entry(combinedLink).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            _context.CustomerLink.Add(combinedLink.Link);
            await _context.SaveChangesAsync();

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

            var customerLink = await _context.CustomerLink.FindAsync(id);
            if (customerLink == null)
            {
                return NotFound();
            }

            _context.CustomerLink.Remove(customerLink);
            await _context.SaveChangesAsync();

            return Ok(customerLink);
        }

        private bool CustomerLinkExists(Guid id)
        {
            return _context.CustomerLink.Any(e => e.ID == id);
        }
    }
}