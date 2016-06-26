using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class BasicStats
{
    public int MaxHealth { get {return maxHealth; } set { maxHealth = value; } }
    public int CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }
    public int Defense { get { return defense; } set { defense = value; } }
    public float DodgeRate { get { return dodgeRate; } set { dodgeRate = value; } }
    public float BlockRate { get { return blockRate; } set { blockRate = value; } }
    public int Attack { get { return attack; } set { attack = value; } }
    public float HitRate { get { return hitRate; } set { hitRate = value; } }
    public float BlockBreakRate { get { return blockBreakRate; } set { blockBreakRate = value; } }
    public float CritRate { get { return critRate; } set { critRate = value; } }
    public float CritDamage { get { return critDamage; } set { critDamage = value; } }

    [SerializeField]
    [Range(1, 10000)]
    private int maxHealth;

    [SerializeField]
    [Range(1, 10000)]
    private int currentHealth;

    [SerializeField]
    [Range(1, 5000)]
    private int defense;

    [SerializeField]
    [Range(0, 100)]
    private float dodgeRate;

    [SerializeField]
    [Range(0, 100)]
    private float blockRate;

    [SerializeField]
    [Range(0, 5000)]
    private int attack;

    [SerializeField]
    [Range(100, 500)]
    private float hitRate;

    [SerializeField]
    [Range(100, 300)]
    private float blockBreakRate;

    [SerializeField]
    [Range(0, 100)]
    private float critRate;

    [SerializeField]
    [Range(0, 500)]
    private float critDamage;


    public BasicStats(int maxHealth, int defense, float dodgeRate, float blockRate, int attack, float hitRate, float blockBreakRate, float critRate, float critDamage)
    {
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        this.defense = defense;
        this.dodgeRate = dodgeRate;
        this.blockRate = blockRate;
        this.attack = attack;
        this.hitRate = hitRate;
        this.blockBreakRate = blockBreakRate;
        this.critRate = critRate;
        this.critDamage = critDamage;
    }

    public static BasicStats PlayerTest()
    {
        return new BasicStats(1500, 50, 25, 40, 100, 120, 110, 50, 50);
    }

    public static BasicStats EnemyTest()
    {
        return new BasicStats(500, 10, 10, 20, 60, 100, 100, 30, 10);
    }

    public enum AttackOutcome
    {
        Blocked,
        Crit,
        Hit,
        Miss
    }
}
