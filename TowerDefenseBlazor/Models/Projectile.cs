namespace TowerDefenseBlazor.Models;

public class Projectile
{
    public float X { get; set; }
    public float Y { get; set; }
    public Enemy Target { get; } // Привязка к объекту врага
    public int Damage { get; }
    public float Speed { get; } = 8f;
    public string Color { get; }

    public Projectile(float x, float y, Enemy target, int damage, string color)
    {
        X = x; Y = y; Target = target; Damage = damage; Color = color;
    }
}