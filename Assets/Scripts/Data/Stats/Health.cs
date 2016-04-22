using UnityEngine;
using System.Collections;

public class Health
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    public Health(int maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public Health (int maxHealth, int currentHealth)
    {
        MaxHealth = maxHealth;
        if (currentHealth > maxHealth)
        {
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth = currentHealth;
        }
    }

    public Health(BasicStats bs)
    {
        MaxHealth = bs.MaxHealth;
        CurrentHealth = bs.CurrentHealth;
    }

    public float HealthPercent()
    {
        return (float)CurrentHealth / MaxHealth;
    }
}
