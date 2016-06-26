using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using PlayerLogic;
using EnemyAI;

public class CombatTextManager : Singleton<CombatTextManager>
{

    [SerializeField]
    private RectTransform canvas;

    [SerializeField]
    private GameObject textPrefab;

    [Space(10)]
    [SerializeField]
    private Vector2 textDirection;
    [Range(0.1f, 3)]
    [SerializeField]
    private float textSpeed;
    [Range(0.1f, 3)]
    [SerializeField]
    private float fadeDelay;

    [Space(20)]
    [SerializeField]
    private Color playerHitTextColor;
    [SerializeField]
    private Color playerCritTextColor;
    [SerializeField]
    private Color playerBlockTextColor;
    [SerializeField]
    private Color playerDodgeTextColor;

    [Space(10)]
    [SerializeField]
    private Color enemyHitTextColor;
    [SerializeField]
    private Color enemyCritTextColor;
    [SerializeField]
    private Color enemyBlockTextColor;
    [SerializeField]
    private Color enemyDodgeTextColor;


    CombatTextManager() { }

    void OnEnable()
    {
        PlayerStateHandler.DamagedPlayer += CreateCombatText;
        EnemyStateHandler.DamagedEnemy += CreateCombatText;
    }
    void OnDisable()
    {
        PlayerStateHandler.DamagedPlayer -= CreateCombatText;
        EnemyStateHandler.DamagedEnemy -= CreateCombatText;
    }


    public void CreateCombatText(GameObject targetGo, int damage, BasicStats.AttackOutcome outcome)
    {
        GameObject combatTextGo = (GameObject)Instantiate(textPrefab, targetGo.transform.position, Quaternion.identity);
        combatTextGo.transform.SetParent(canvas, false);
        combatTextGo.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(GetTargetTopPosition(targetGo));

        combatTextGo.GetComponent<CombatText>().SetTextAttributs(textDirection, textSpeed, fadeDelay);
        SetTextProperties(combatTextGo.GetComponent<Text>(), combatTextGo.GetComponent<Animator>(), targetGo.tag, damage, outcome);

        combatTextGo.SetActive(true);
    }

    private Vector2 GetTargetTopPosition(GameObject targetGo)
    {
        SpriteRenderer spr = targetGo.GetComponent<SpriteRenderer>();
        return spr.bounds.center + new Vector3(0, spr.bounds.extents.y);
    }

    private void SetTextProperties(Text textComponent, Animator anim, string targetTag, int damage, BasicStats.AttackOutcome outcome)
    {
        Color chosenColor;
        string content;
        if (targetTag == "Player")
        {
            switch (outcome)
            {
                case BasicStats.AttackOutcome.Blocked:
                    chosenColor = playerBlockTextColor;
                    content = "Block";
                    break;
                case BasicStats.AttackOutcome.Crit:
                    chosenColor = playerCritTextColor;
                    content = damage.ToString();
                    anim.SetTrigger("Crit");
                    break;
                case BasicStats.AttackOutcome.Hit:
                    chosenColor = playerHitTextColor;
                    content = damage.ToString();
                    break;
                case BasicStats.AttackOutcome.Miss:
                    chosenColor = playerDodgeTextColor;
                    content = "Dodge";
                    break;
                default:
                    Debug.Log("There's no state in this name :" + outcome);
                    chosenColor = Color.white;
                    content = "None";
                    break;
            }

        }
        else
        {
            switch (outcome)
            {
                case BasicStats.AttackOutcome.Blocked:
                    chosenColor = enemyBlockTextColor;
                    content = "Block";
                    break;
                case BasicStats.AttackOutcome.Crit:
                    chosenColor = enemyCritTextColor;
                    content = damage.ToString();
                    anim.SetTrigger("Crit");
                    break;
                case BasicStats.AttackOutcome.Hit:
                    chosenColor = enemyHitTextColor;
                    content = damage.ToString();
                    break;
                case BasicStats.AttackOutcome.Miss:
                    chosenColor = enemyDodgeTextColor;
                    content = "Miss";
                    break;
                default:
                    Debug.Log("There's no state in this name :" + outcome);
                    chosenColor = Color.white;
                    content = "None";
                    break;
            }

        }

        textComponent.text = content;
        textComponent.color = chosenColor;
    }

}
