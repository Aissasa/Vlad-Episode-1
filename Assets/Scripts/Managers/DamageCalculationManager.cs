using UnityEngine;
using System.Collections;
using System;

public class DamageCalculationManager : Singleton<DamageCalculationManager> {

    private DamageCalculationManager() { }

    public int CalculateInflictedDamage(BasicStats attackerStats, BasicStats defenderStats, out BasicStats.AttackOutcome outcome)
    {
        if (Dodge(attackerStats.hitRate, defenderStats.dodgeRate))
        {
            outcome = BasicStats.AttackOutcome.Miss;
            return 0;
        }
        if (Block(attackerStats.blockBreakRate, defenderStats.blockRate))
        {
            outcome = BasicStats.AttackOutcome.Blocked;
            return BlockDamageCalc(attackerStats.attack, defenderStats.defense);
        }
        if (Crit(attackerStats.critRate))
        {
            outcome = BasicStats.AttackOutcome.Crit;
            return CritDamageCalc(attackerStats.attack, defenderStats.defense, attackerStats.critDamage);
        }
        outcome = BasicStats.AttackOutcome.Hit;
        return DamageCalc(attackerStats.attack, defenderStats.defense);
    }

    protected bool Block(float blockBreakRate, float blockRate)
    {
        return UnityEngine.Random.Range(0, blockBreakRate) < UnityEngine.Random.Range(0, blockRate);
    }

    protected int BlockDamageCalc(int attack, int defense)
    {
        return DamageCalc(attack, defense) / 4;
    }

    // todo : crit shouldn't reach very high values
    protected bool Crit(float critRate)
    {
        return UnityEngine.Random.Range(0, 101) < UnityEngine.Random.Range(0, critRate);

    }
    protected int CritDamageCalc(int attack, int defense, float critDmg)
    {
        return Mathf.RoundToInt(DamageCalc(attack, defense) * (1 + critDmg/100));
    }

    protected int DamageCalc(int attack, int defense)
    {
        int dmg = attack - defense;
        if (dmg <= 0)
        {
            dmg = 1;
        }
        return dmg;
    }

    protected bool Dodge(float hitRate, float dodgeRate)
    {
        return UnityEngine.Random.Range(0, hitRate) < UnityEngine.Random.Range(0, dodgeRate);
    }
}
