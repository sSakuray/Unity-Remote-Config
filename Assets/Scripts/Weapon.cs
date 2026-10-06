using UnityEngine;

public sealed class Weapon : MonoBehaviour
{
    public string id = "pistol";
    public int damage = 10;
    public float cooldown = 0.5f;

    public void Apply(int newDamage, float newCooldown)
    {
        damage = newDamage;
        cooldown = newCooldown;
        Debug.Log($"Weapon {id}: damage={damage}, cooldown={cooldown}", this);
    }
}

