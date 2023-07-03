using LoginApi.Data;
using LoginApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class TotalScoresController : ControllerBase
{
    private readonly UserContext _context;
    private readonly ILogger<TotalScoresController> _logger;
    private readonly IConfiguration _configuration;

    public TotalScoresController(UserContext context, ILogger<TotalScoresController> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> PostTotalScore(TotalScore totalScore)
    {
        var nameIdentifierClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null)
        {
            return BadRequest("No user is currently logged in.");
        }

        if (!int.TryParse(nameIdentifierClaim.Value, out var loggedInUserId))
        {
            return BadRequest("Invalid user ID.");
        }

        totalScore.UserId = loggedInUserId;

        _context.TotalScores.Add(totalScore);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTotalScoreByUser), new { id = totalScore.Id }, totalScore);
    }


    [HttpGet("user")]
    public async Task<ActionResult<TotalScore>> GetTotalScoreByUser()
    {
        // Extract the user ID from the JWT token
        var nameIdentifierClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null)
        {
            return BadRequest("No user is currently logged in.");
        }

        if (!int.TryParse(nameIdentifierClaim.Value, out var loggedInUserId))
        {
            return BadRequest("Invalid user ID.");
        }

        // Use the user ID to find the TotalScore
        var totalScore = await _context.TotalScores.FirstOrDefaultAsync(score => score.UserId == loggedInUserId);

        if (totalScore == null)
        {
            return NotFound();
        }

        return totalScore;
    }

    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<TotalScore>>> GetAllTotalScores()
    {
        return await _context.TotalScores.ToListAsync();
    }

}
