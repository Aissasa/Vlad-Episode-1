using UnityEngine;
using System.Collections;

public class BasicStats {

    public int maxHealth { get; set; }
    public int currentHealth { get; set; }
    public int defense { get; set; }
    public float dodgeRate { get; set; }
    public float blockRate { get; set; }
    public int attack { get; set; }
    public float hitRate { get; set; }
    public float critRate { get; set; }
    public float critDamage { get; set; }

    public BasicStats(int _maxHealth, int _currentHealth, int _defense, float _dodgeRate, float _blockRate, int _attack, float _hitRate, float _critRate, float _critDamage)
    {
        maxHealth = _maxHealth;
        currentHealth = _currentHealth;
        if (currentHealth == 0)
        {
            currentHealth = maxHealth;
        }
        defense = _defense;
        dodgeRate = _dodgeRate;
        blockRate = _blockRate;
        attack = _attack;
        hitRate = _hitRate;
        critRate = _critRate;
        critDamage = _critDamage;
    }
}
