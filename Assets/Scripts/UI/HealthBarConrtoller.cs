using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayerLogic;

public class HealthBarConrtoller : MonoBehaviour
{

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Text healthBarText;

    void Start()
    {
        Health health = GameManager.Instance.GetCurrentPlayerHealth();
        healthBarText.text = "Health : " + health.CurrentHealth + " / " + health.MaxHealth;

    }

    void PlayerHit(Health health)
    {
        healthBar.fillAmount = health.HealthPercent();
        healthBarText.text = "Health : " + health.CurrentHealth + " / " + health.MaxHealth;
    }

    void OnEnable()
    {
        PlayerStateHandler.HitPlayer += PlayerHit;
    }

    void OnDisable()
    {
        PlayerStateHandler.HitPlayer -= PlayerHit;
    }


}
