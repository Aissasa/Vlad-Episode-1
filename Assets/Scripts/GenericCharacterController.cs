using UnityEngine;
using System.Collections;

public abstract class GenericCharacterController : MonoBehaviour {

    protected Animator animator;
    public Transform spawnPosition;       // the position of first spawn
    protected Vector2 mouvementVector;     
    protected float characterSpeed;       // speed for normal mouvement
    protected bool isFacingRight;         // bool to know which direction the player is facing
    private bool isInBlockingAnimation;   // bool to stop the mouvement in blocking animations

    // animator's triggers and bools names
    protected string attackingAnimationTrigger = "attacking";
    protected string dyingAnimationTrigger = "dying";             //not used yet
    protected string hitAnimationTrigger = "hit";                 //not used yet
    protected string walkingAnimationBool = "walking";

    protected enum MyAnimationState
    {
        Idle,
        Walk,
        Attack,
        Hit,
        Dead
    }

    // abstract methods
    protected abstract void AssignToSpawnPosition();
    protected abstract void AttackFunc();
    protected abstract void GotHit();
    protected abstract void GotKilled();
    protected abstract void UpdateMouvementVector();

    protected virtual void Start()
    {
        AssignToSpawnPosition(); // todo find a solution for spawn position

        animator = GetComponent<Animator>();
        characterSpeed = 1.5f;
        isFacingRight = true;
        isInBlockingAnimation = false;
    }

    protected virtual void Update()
    {
        CanItMove();
        UpdateMouvementVector();
        MoveCharacter();
        AttackFunc();
        GotHit(); // urgent : need to implement gothit and gotkilled ASAP
        GotKilled();
    }

    protected void CanItMove()
    {
        MyAnimationState animState = GetAnimationState();

        switch (animState)
        {
            case MyAnimationState.Attack:
                isInBlockingAnimation = true;
                break;
            case MyAnimationState.Dead:
                isInBlockingAnimation = true;
                break;
            case MyAnimationState.Hit:
                isInBlockingAnimation = true;
                break;
            default:
                isInBlockingAnimation = false;
                break;
        }
    }

    protected void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    protected MyAnimationState GetAnimationState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return MyAnimationState.Attack;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            return MyAnimationState.Walk;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            return MyAnimationState.Dead;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return MyAnimationState.Dead;
        }
        else
        {
            return MyAnimationState.Idle;
        }

    }

    protected virtual void MoveCharacter()
    {
        if (!isInBlockingAnimation && Vector2.zero != mouvementVector)
        {
            if ((mouvementVector.x > 0 && !isFacingRight) || (mouvementVector.x < 0 && isFacingRight))
            {
                Flip();
            }
            animator.SetBool(walkingAnimationBool, true);
            LinearMouvement.Instance().Move(gameObject, mouvementVector, characterSpeed);
        }
        else
        {
            animator.SetBool(walkingAnimationBool, false);
        }
    }
}
