namespace TowerDefenseBlazor.Models;

public class Projectile
{
    public float X { get; set; }
    public float Y { get; set; }
    public float TargetX { get; }
    public float TargetY { get; }
    public int Damage { get; }
    public float Speed { get; } = 8f;
    public string Color { get; }

    public Projectile(float x, float y, float tx, float ty, int damage, string color)
    {
        X = x; Y = y; TargetX = tx; TargetY = ty; Damage = damage; Color = color;
    }
}