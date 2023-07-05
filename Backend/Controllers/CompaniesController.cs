using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace LoginApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ILogger<CompaniesController> _logger;
        private readonly IConfiguration _configuration;

        public CompaniesController(UserContext context, ILogger<CompaniesController> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Company>> PostCompany(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanies", new { id = company.Id }, company);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpGet("{companyId}/users")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersForCompany(int companyId)
        {
            var users = await _context.Users.Where(u => u.CompanyId == companyId).ToListAsync();

            if (users == null || users.Count == 0)
            {
                return Ok(new List<User>());
            }

            return users;
        }

        // CompaniesController.cs
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteCompany(int id)
{
    var company = await _context.Companies.FindAsync(id);
    if (company == null)
    {
        return NotFound();
    }

    // Remove all users associated with this company
    var users = await _context.Users.Where(u => u.CompanyId == id).ToListAsync();
    _context.Users.RemoveRange(users);

    // Remove the company itself
    _context.Companies.Remove(company);
    await _context.SaveChangesAsync();

    return NoContent();
}

    }

    
}
