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

    // Очередь для постепенного спавна
    private readonly Queue<Enemy> _spawnQueue = new();
    // Интервал между спавном отдельных врагов (в секундах)
    private const float SpawnInterval = 1.0f;

    public readonly Vector2[] Path = {
        new(0, 300), new(200, 300), new(200, 100), new(500, 100),
        new(500, 400), new(700, 400), new(700, 250), new(900, 250)
    };

    private readonly Random _rand = new();

    public void SpawnWave()
    {
        _spawnQueue.Clear();
        int enemiesCount = Wave * 3;

        for (int i = 0; i < enemiesCount; i++)
        {
            var type = _rand.Next(3);
            Enemy e = type switch
            {
                0 => new FastEnemy(-30, 300),
                1 => new NormalEnemy(-30, 300),
                _ => new TankEnemy(-30, 300)
            };

            // Рассчитываем ХП с учетом волны
            int calculatedHp = type switch
            {
                0 => 15 + (Wave * 3),
                1 => 40 + (Wave * 8),
                _ => 100 + (Wave * 20)
            };

            // Присваиваем оба значения, чтобы полоска была заполненной при спавне
            e.HP = calculatedHp;
            e.MaxHP = calculatedHp;

            _spawnQueue.Enqueue(e);
        }

        SpawnTimer = 0;
        Wave++;
    }

    public void Update(float deltaTime)
    {
        // Постепенный спавн из очереди
        if (_spawnQueue.Count > 0)
        {
            SpawnTimer -= deltaTime;
            if (SpawnTimer <= 0)
            {
                Enemies.Add(_spawnQueue.Dequeue());
                SpawnTimer = SpawnInterval; // Ждем следующего
            }
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
                // Передаем саму цель (target) вместо координат
                Projectiles.Add(new Projectile(tower.X, tower.Y, target, tower.Damage, tower.Color));
                tower.CurrentCooldown = tower.Cooldown;
            }
        }

        // Движение снарядов + попадание
        foreach (var p in Projectiles.ToList())
        {
            // Если враг уже мертв или ушел, пока летел снаряд, просто удаляем снаряд
            if (p.Target == null || !Enemies.Contains(p.Target))
            {
                Projectiles.Remove(p);
                continue;
            }

            // Летим строго за текущей позицией врага
            var dx = p.Target.X - p.X;
            var dy = p.Target.Y - p.Y;
            var dist = MathF.Sqrt(dx * dx + dy * dy);

            if (dist < 12) // Попадание
            {
                p.Target.HP -= p.Damage;
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

    // Вспомогательное свойство для Game.razor, чтобы кнопка "Начать волну" блокировалась правильно
    public bool IsWaveActive => _spawnQueue.Count > 0 || Enemies.Count > 0;
}