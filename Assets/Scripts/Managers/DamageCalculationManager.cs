using UnityEngine;
using System.Collections;
using System;

public class DamageCalculationManager : Singleton<DamageCalculationManager> {

    private DamageCalculationManager() { }

    public int CalculateInflictedDamage(BasicStats attackerStats, BasicStats defenderStats, out BasicStats.AttackOutcome outcome)
    {
        if (Dodge(attackerStats.HitRate, defenderStats.DodgeRate))
        {
            outcome = BasicStats.AttackOutcome.Miss;
            return 0;
        }
        if (Block(attackerStats.BlockBreakRate, defenderStats.BlockRate))
        {
            outcome = BasicStats.AttackOutcome.Blocked;
            return BlockDamageCalc(attackerStats.Attack, defenderStats.Defense);
        }
        if (Crit(attackerStats.CritRate))
        {
            outcome = BasicStats.AttackOutcome.Crit;
            return CritDamageCalc(attackerStats.Attack, defenderStats.Defense, attackerStats.CritDamage);
        }
        outcome = BasicStats.AttackOutcome.Hit;
        return DamageCalc(attackerStats.Attack, defenderStats.Defense);
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
        int roll = UnityEngine.Random.Range(8, 12);
        int dmg = (attack / defense) * roll;
        //int dmg = attack - defense;
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
