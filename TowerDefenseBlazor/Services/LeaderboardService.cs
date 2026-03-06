using System.Net.Http.Json;
using TowerDefenseBlazor.Models;

namespace TowerDefenseBlazor.Services;

public interface ILeaderboardService
{
    Task SubmitScoreAsync(string playerName, int wave);
    Task<List<LeaderboardEntry>> GetTopScoresAsync(int count = 10);
}

public class LeaderboardService : ILeaderboardService
{
    private readonly HttpClient _http;

    public LeaderboardService(HttpClient http)
    {
        _http = http;
    }

    public async Task SubmitScoreAsync(string playerName, int wave)
    {
        var submission = new { PlayerName = playerName, Wave = wave };

        //  Используем ОТНОСИТЕЛЬНЫЙ путь (без домена и порта!)
        var response = await _http.PostAsJsonAsync("api/leaderboard", submission);

        // Проверяем успешность запроса (важно для отладки!)
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<LeaderboardEntry>> GetTopScoresAsync(int count = 10)
    {
        // Тоже относительный путь
        return await _http.GetFromJsonAsync<List<LeaderboardEntry>>(
            $"api/leaderboard/top?count={count}")
            ?? new List<LeaderboardEntry>();
    }
}