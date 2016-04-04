using UnityEngine;
using System.Collections;

public abstract class GenericCharacterController : MonoBehaviour {

    public Transform spawnPosition;       // the position of first spawn
    protected Animator animator;
    protected Vector2 movementVector;     
    protected float characterSpeed;       // speed for normal mouvement
    protected bool isFacingRight;         // bool to know which direction the character is facing
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
    protected abstract void AttackFunc();
    protected abstract void GotHit();
    protected abstract void GotKilled();
    protected abstract void Move();

    protected virtual void Start()
    {
        Spawn();
        animator = GetComponent<Animator>();
        isInBlockingAnimation = false;
        isFacingRight = true;
    }

    protected virtual void Update()
    { 
        CheckCanItMove();
    }

    protected void CheckCanItMove()
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
        if (!isInBlockingAnimation && Vector2.zero != movementVector)
        {
            if ((movementVector.x > 0 && !isFacingRight) || (movementVector.x < 0 && isFacingRight))
            {
                Flip();
            }
            animator.SetBool(walkingAnimationBool, true);
            Move();
        }
        else
        {
            animator.SetBool(walkingAnimationBool, false);
        }
    }

    protected virtual void Spawn()
    {
        if (spawnPosition != null)
        {
            transform.position = spawnPosition.position;
        }
    }
}
