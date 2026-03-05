namespace TowerDefenseBlazor.Models;

public abstract class Enemy
{
    public float X { get; set; }
    public float Y { get; set; }
    public int HP { get; set; }
    public float Speed { get; set; }
    public int GoldReward { get; set; }
    public int CurrentWaypoint { get; set; } = 0;
    public string Color { get; protected set; } = "red";

    protected Enemy(float x, float y, int hp, float speed, int reward, string color)
    {
        X = x; Y = y; HP = hp; Speed = speed; GoldReward = reward; Color = color;
    }

    public bool IsDead() => HP <= 0;
}

public class FastEnemy : Enemy
{
    public FastEnemy(float x, float y) : base(x, y, 5, 3.5f, 15, "orange") { }
}

public class NormalEnemy : Enemy
{
    public NormalEnemy(float x, float y) : base(x, y, 40, 2f, 20, "red") { }
}

public class TankEnemy : Enemy
{
    public TankEnemy(float x, float y) : base(x, y, 100, 1f, 30, "purple") { }
}