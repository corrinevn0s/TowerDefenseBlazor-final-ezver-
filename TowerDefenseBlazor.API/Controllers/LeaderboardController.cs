using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TowerDefenseBlazor.API.Data;
using TowerDefenseBlazor.API.Models;

namespace TowerDefenseBlazor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public LeaderboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/leaderboard
    [HttpPost]
    public async Task<ActionResult<LeaderboardEntry>> SubmitScore([FromBody] ScoreSubmission submission)
    {
        if (string.IsNullOrWhiteSpace(submission.PlayerName) || submission.Wave <= 0)
        {
            return BadRequest(new { message = "Invalid data" });
        }

        var entry = new LeaderboardEntry
        {
            PlayerName = submission.PlayerName.Trim(),
            MaxWave = submission.Wave,
            AchievedAt = DateTime.UtcNow
        };

        _context.LeaderboardEntries.Add(entry);
        await _context.SaveChangesAsync();

        return CreatedAtRoute(
            routeName: null, // null = использовать атрибуты маршрутизации
            routeValues: new { }, // нет параметров в маршруте для GetTopScores
            value: entry);
    }

    // GET: api/leaderboard/top?count=10
    [HttpGet("top")]
    public async Task<ActionResult<IEnumerable<LeaderboardEntry>>> GetTopScores([FromQuery] int count = 10)
    {
        var topScores = await _context.LeaderboardEntries
            .OrderByDescending(e => e.MaxWave)
            .ThenBy(e => e.AchievedAt)
            .Take(count)
            .ToListAsync();

        // Добавляем позиции
        var result = topScores.Select((entry, index) => new LeaderboardEntry
        {
            Id = index + 1,
            PlayerName = entry.PlayerName,
            MaxWave = entry.MaxWave,
            AchievedAt = entry.AchievedAt
        }).ToList();

        return Ok(result);
    }

    // DTO для получения данных
    public class ScoreSubmission
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Wave { get; set; }
    }
}