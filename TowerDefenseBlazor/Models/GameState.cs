using TowerDefenseBlazor.Models;

namespace TowerDefenseBlazor.Models;

public class GameState
{
    public List<Enemy> Enemies { get; } = new();
    public List<Tower> Towers { get; } = new();
    public List<Projectile> Projectiles { get; } = new();
    public int Gold { get; set; } = 300;
    public int Lives { get; set; } = 10;
    public int Wave { get; set; } = 1;
    public float SpawnTimer { get; set; } = 0;

    // Фиксированный путь (можно изменить)
    public readonly Vector2[] Path = {
        new(0, 300), new(200, 300), new(200, 100), new(500, 100),
        new(500, 400), new(700, 400), new(700, 250), new(900, 250)
    };

    private readonly Random _rand = new();

    public void SpawnWave()
    {
        for (int i = 0; i < Wave * 3; i++)
        {
            var type = _rand.Next(3);
            Enemy e = type == 0 ? new FastEnemy(-30, 300) :
                      type == 1 ? new NormalEnemy(-30, 300) : new TankEnemy(-30, 300);
            Enemies.Add(e);
        }
        Wave++;
    }

    public void Update(float deltaTime)
    {
        // Спавн
        SpawnTimer -= deltaTime;
        if (SpawnTimer <= 0 && Enemies.Count < Wave * 3)
        {
            SpawnTimer = 0.8f;
            // спавним по одному (для простоты)
        }

        // Движение врагов
        foreach (var enemy in Enemies.ToList())
        {
            if (enemy.CurrentWaypoint >= Path.Length - 1)
            {
                Lives--;
                Enemies.Remove(enemy);
                continue;
            }

            var target = Path[enemy.CurrentWaypoint + 1];
            var dx = target.X - enemy.X;
            var dy = target.Y - enemy.Y;
            var dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist < 5)
            {
                enemy.CurrentWaypoint++;
            }
            else
            {
                enemy.X += (dx / dist) * enemy.Speed;
                enemy.Y += (dy / dist) * enemy.Speed;
            }
        }

        // Башни стреляют
        foreach (var tower in Towers)
        {
            tower.CurrentCooldown -= deltaTime;
            if (!tower.CanShoot()) continue;

            var target = Enemies.FirstOrDefault(e =>
                MathF.Sqrt(MathF.Pow(e.X - tower.X, 2) + MathF.Pow(e.Y - tower.Y, 2)) < tower.Range);

            if (target != null)
            {
                Projectiles.Add(new Projectile(tower.X, tower.Y, target.X, target.Y, tower.Damage, tower.Color));
                tower.CurrentCooldown = tower.Cooldown;
            }
        }

        // Движение снарядов + попадание
        foreach (var p in Projectiles.ToList())
        {
            var dx = p.TargetX - p.X;
            var dy = p.TargetY - p.Y;
            var dist = MathF.Sqrt(dx * dx + dy * dy);
            if (dist < 10)
            {
                var hit = Enemies.FirstOrDefault(e =>
                    MathF.Abs(e.X - p.TargetX) < 20 && MathF.Abs(e.Y - p.TargetY) < 20);
                if (hit != null) hit.HP -= p.Damage;
                Projectiles.Remove(p);
                continue;
            }
            p.X += (dx / dist) * p.Speed;
            p.Y += (dy / dist) * p.Speed;
        }

        // Удаляем мёртвых
        foreach (var e in Enemies.Where(e => e.IsDead()).ToList())
        {
            Gold += e.GoldReward;
            Enemies.Remove(e);
        }
    }
}