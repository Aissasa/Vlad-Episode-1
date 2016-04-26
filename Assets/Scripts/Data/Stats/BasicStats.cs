using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BasicStats
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Defense { get; set; }
    public float DodgeRate { get; set; }
    public float BlockRate { get; set; }
    public int Attack { get; set; }
    public float HitRate { get; set; }
    public float BlockBreakRate { get; set; }
    public float CritRate { get; set; }
    public float CritDamage { get; set; }

    public BasicStats(int maxHealth, int defense, float dodgeRate, float blockRate, int attack, float hitRate, float blockBreakRate, float critRate, float critDamage)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
        Defense = defense;
        DodgeRate = dodgeRate;
        BlockRate = blockRate;
        Attack = attack;
        HitRate = hitRate;
        BlockBreakRate = blockBreakRate;
        CritRate = critRate;
        CritDamage = critDamage;
    }

    public static BasicStats PlayerTest()
    {
        return new BasicStats(100, 50, 25, 40, 100, 120, 110, 50, 50);
    }

    public static BasicStats EnemyTest()
    {
        return new BasicStats(500, 10, 10, 20, 60, 100, 100, 10, 10);
    }

    public enum AttackOutcome
    {
        Blocked,
        Crit,
        Hit,
        Miss
    }
}
