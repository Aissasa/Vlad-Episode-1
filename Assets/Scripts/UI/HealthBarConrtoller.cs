using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PlayerLogic;

public class HealthBarConrtoller : MonoBehaviour
{

    [SerializeField]
    private Image healthBar;

    void PlayerHit(Health health)
    {
        healthBar.fillAmount = health.HealthPercent();
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
