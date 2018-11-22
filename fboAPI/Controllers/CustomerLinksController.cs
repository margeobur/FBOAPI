using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fboAPI.Models;

namespace fboAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerLinksController : ControllerBase
    {
        private readonly LinksContext _context;

        public CustomerLinksController(LinksContext context)
        {
            _context = context;
        }

        // GET: api/CustomerLinks
        [HttpGet]
        public IEnumerable<CustomerLink> GetCustomerLink()
        {
            return _context.CustomerLink;
        }

        // GET: api/CustomerLinks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerLink([FromRoute] Guid id)
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
        public async Task<IActionResult> PutCustomerLink([FromRoute] Guid id, [FromBody] CustomerLink customerLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerLink.ID)
            {
                return BadRequest();
            }

            _context.Entry(customerLink).State = EntityState.Modified;

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
        public async Task<IActionResult> PostCustomerLink([FromBody] CustomerLink customerLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CustomerLink.Add(customerLink);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomerLink", new { id = customerLink.ID }, customerLink);
        }

        // DELETE: api/CustomerLinks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerLink([FromRoute] Guid id)
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