namespace TowerDefenseBlazor.API.Models;

public class LeaderboardEntry
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int MaxWave { get; set; }
    public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
}