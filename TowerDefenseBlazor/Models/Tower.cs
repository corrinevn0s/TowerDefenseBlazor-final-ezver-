namespace TowerDefenseBlazor.Models;

public abstract class Tower
{
    public float X { get; set; }
    public float Y { get; set; }
    public int Cost { get; }
    public float Range { get; }
    public int Damage { get; }
    public float Cooldown { get; } // в секундах
    public float CurrentCooldown { get; set; } = 0;
    public string Color { get; protected set; }

    protected Tower(float x, float y, int cost, float range, int damage, float cooldown, string color)
    {
        X = x; Y = y; Cost = cost; Range = range; Damage = damage; Cooldown = cooldown; Color = color;
    }

    public bool CanShoot() => CurrentCooldown <= 0;
}

public class BowTower : Tower
{
    public BowTower(float x, float y) : base(x, y, 50, 150, 15, 0.6f, "brown") { }
}

public class FireTower : Tower
{
    public FireTower(float x, float y) : base(x, y, 100, 120, 25, 1.2f, "orange") { }
}

public class PoisonTower : Tower
{
    public PoisonTower(float x, float y) : base(x, y, 80, 140, 10, 0.8f, "green") { }
}