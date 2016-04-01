using UnityEngine;
using System.Collections;
using System;

public class PlayerController : GenericCharacterController
{
    protected override void Start()
    {
        base.Start();
        isFacingRight = true;
        characterSpeed = 1.5f;
    }

    protected override void Update()
    {
        base.Update();
        UpdateMovementVector();
        MoveCharacter();
        AttackFunc();
        GotHit(); // todo : need to implement gothit and gotkilled ASAP
        GotKilled();

    }

    protected override void AttackFunc()
    {
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

    protected override void Move()
    {
        LinearMouvement.Instance().MoveTowards(gameObject, movementVector, characterSpeed);
    }

    void UpdateMovementVector()
    {
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
