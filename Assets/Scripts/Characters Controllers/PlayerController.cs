using UnityEngine;
using System.Collections;
using System;

public class PlayerController : GenericCharacterController
{
    public delegate void MovementAction();
    public static event MovementAction Moved;

    Vector2 positionOfTheLastMovedTrigger;
    bool hasMoved;
    float movedTriggerCounter;
    [Range(0.01f, 2)]
    public float chasingDelay;
    
    protected override void Start()
    {
        base.Start();
        //characterSpeed = 1.5f;
        positionOfTheLastMovedTrigger = transform.Get2DPosition();
        hasMoved = false;
        movedTriggerCounter = chasingDelay;
    }

    protected override void Update()
    {
        base.Update();
        MoveCharacter();
        AttackFunc();
        GotHit(); 
        GotKilled();

    }

    protected void LateUpdate()
    {
        TriggerChasingEvent();
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
        LinearMouvement.Instance.MoveTowardsDirection(gameObject, movementVector, characterSpeed);
    }

    protected void CheckIfHasMoved()
    {
        if (!hasMoved && transform.Get2DPosition() != positionOfTheLastMovedTrigger) 
        {
            hasMoved = true;
        }
    }

    protected void TriggerChasingEvent()
    {
        if (Moved == null)
        {
            return;
        }
        CheckIfHasMoved();
        if (hasMoved && movedTriggerCounter <= 0)
        {
            positionOfTheLastMovedTrigger = transform.Get2DPosition();
            hasMoved = false;
            movedTriggerCounter = chasingDelay;
            Moved();
        }
        else
        {
            movedTriggerCounter -= Time.deltaTime;
        }
    }

    protected override void UpdateMovementVector()
    {
        movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
