using Xunit;
using TowerDefenseBlazor.Models;
using System.Linq;

namespace TowerDefenseBlazor.Tests;

public class GameStateTests
{
    [Fact]
    public void Test1_SpawnWave_FillsQueue_AndSpawnsGradually()
    {
        // Arrange
        var gameState = new GameState();

        // Act & Assert: Изначально врагов нет
        Assert.Empty(gameState.Enemies);

        // Запускаем волну
        gameState.SpawnWave();

        // Так как SpawnTimer инициализируется в 0, первый апдейт сразу выдернет 1 врага из очереди
        gameState.Update(0.016f);

        // Проверяем: На поле появился ровно один враг, остальные ждут в очереди
        Assert.Single(gameState.Enemies);
        Assert.True(gameState.IsWaveActive, "Волна должна быть активна, пока очередь или список врагов не пусты");
    }

    [Fact]
    public void Test2_Enemy_HealthBar_CorrectPercentages()
    {
        // Arrange
        var fastEnemy = new FastEnemy(0, 0);
        fastEnemy.HP = 20;
        fastEnemy.MaxHP = 20;

        // Act: Наносим урон
        fastEnemy.HP -= 5;

        // Assert: Проверяем пропорции для полоски здоровья
        Assert.Equal(20, fastEnemy.MaxHP);
        Assert.Equal(15, fastEnemy.HP);

        float hpPercent = (float)fastEnemy.HP / fastEnemy.MaxHP;
        Assert.Equal(0.75f, hpPercent); // Оставшиеся 75% полоски здоровья
    }

    [Fact]
    public void Test3_Projectiles_TrackEnemyTarget_AndDealDamage()
    {
        // Arrange
        var gameState = new GameState();
        var enemy = new NormalEnemy(100, 100) { HP = 40, MaxHP = 40 };
        gameState.Enemies.Add(enemy);

        // Создаем снаряд, летящий во врага (урон 15)
        var projectile = new Projectile(90, 100, enemy, 15, "brown");
        gameState.Projectiles.Add(projectile);

        // Act: Обновляем состояние игры. Снаряд находится очень близко (дистанция 10px), 
        // поэтому при вызове Update должно произойти попадание.
        gameState.Update(0.016f);

        // Assert
        Assert.Equal(25, enemy.HP); // 40 - 15 = 25 HP осталось
        Assert.Empty(gameState.Projectiles); // Снаряд должен удалиться после попадания
    }

    [Fact]
    public void Test4_PathBlocker_Validation_Logic()
    {
        // Arrange
        var gameState = new GameState();
        // Наш путь включает вертикальный отрезок: с (200, 300) до (200, 100)
        // Проверим точку (200, 200), которая находится ровно посередине этого отрезка
        float mx = 200f;
        float my = 200f;
        float roadRadius = 40f;
        bool onPath = false;

        // Act: Запускаем тот же цикл проверки, что и в методе HandleMouseDown в Game.razor
        for (int i = 0; i < gameState.Path.Length - 1; i++)
        {
            var p1 = gameState.Path[i];
            var p2 = gameState.Path[i + 1];

            float minX = MathF.Min(p1.X, p2.X) - roadRadius;
            float maxX = MathF.Max(p1.X, p2.X) + roadRadius;
            float minY = MathF.Min(p1.Y, p2.Y) - roadRadius;
            float maxY = MathF.Max(p1.Y, p2.Y) + roadRadius;

            if (mx >= minX && mx <= maxX && my >= minY && my <= maxY)
            {
                if (p1.X == p2.X) // Вертикальный участок
                {
                    if (MathF.Abs(mx - p1.X) < roadRadius) { onPath = true; break; }
                }
            }
        }

        // Assert
        Assert.True(onPath, "Точка на середине прямой линии должна быть распознана как дорога");!
    }

    [Fact]
    public void Test5_EnemyDeath_AwardsGold_AndRemovesFromList()
    {
        // Arrange
        var gameState = new GameState();
        gameState.Gold = 100;

        var enemy = new FastEnemy(100, 100) { HP = 10, GoldReward = 15 };
        gameState.Enemies.Add(enemy);

        // Act: Смертельный урон
        enemy.HP -= 15; // ХП уходит в минус (-5)

        // Вызываем обновление игры, чтобы сработал триггер очистки мертвецов
        gameState.Update(0.016f);

        // Assert
        Assert.Empty(gameState.Enemies); // Враг удален с карты
        Assert.Equal(115, gameState.Gold); // Золото успешно зачислено (100 + 15)
    }
}