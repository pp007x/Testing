using System.Linq;
using System.Collections.Generic;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using LoginApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly UserContext _context;

        public LinkController(UserContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<Link>> GetLink()
        {
            var usernameClaim = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameClaim);

            if (user == null)
            {
                return NotFound();
            }

            var link = await _context.Links.FindAsync(user.CompanyId);
            if (link == null)
            {
                return NotFound();
            }

            return link;
}

[HttpPost]
[Authorize(Roles = "Admin")]
public async Task<ActionResult<Link>> PostLink(Link link)
{
    try
    {
        var existingLink = await _context.Links.FirstOrDefaultAsync(l => l.CompanyId == link.CompanyId);
        if (existingLink != null)
        {
            // If a link with the same companyId exists, update the existing link
            _context.Entry(existingLink).CurrentValues.SetValues(link);
            existingLink.Id = existingLink.Id; // Ensure the ID remains the same
        }
        else
        {
            // If no link with the same companyId exists, add the new link
            _context.Links.Add(link);
        }
        await _context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        // Handle any other exceptions that might occur
        return BadRequest(ex.Message);
    }

    return CreatedAtAction("GetLink", new { id = link.Id }, link);
}


    }
}
