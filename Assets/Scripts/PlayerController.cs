using UnityEngine;
using System.Collections;
using System;

public class PlayerController : GenericCharacterController
{
    protected override void AssignToSpawnPosition()
    {
        gameObject.transform.position = spawnPosition.position;
    }

    protected override void AttackFunc()
    {
        //either with one press or continuous press trigger the attack
        if (Input.GetButton("Attack"))
        {
            animator.SetTrigger(attackingAnimationTrigger);
        }

        if (Input.GetButtonUp("Attack"))
        {
            animator.ResetTrigger(attackingAnimationTrigger);
        }
    }

    protected override void GotHit()
    {
        if (Input.GetButton("Fire2"))
        {
            animator.SetTrigger(hitAnimationTrigger);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            animator.ResetTrigger(hitAnimationTrigger);
        }

    }

    protected override void GotKilled()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            animator.SetTrigger(dyingAnimationTrigger);
        }
    }

    protected override void UpdateMouvementVector()
    {
        mouvementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
