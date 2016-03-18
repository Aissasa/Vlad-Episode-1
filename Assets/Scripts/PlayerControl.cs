using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    Rigidbody2D rb2d;
    Animator animator;

    enum animationState
    {
        Idle,
        Walk,
        Attack,
        Dead
    }

    // the position of first spawn
    Vector3 firstPosition;
    float playerLateralSpeed; // speed for lateral mouvement
    float playerSpeed; // speed for normal mouvement
    bool isFacingRight;
    bool isAttacking;  // bool to stop the mouvement in the attacking animation

    // animator's triggers and bools names
    string attackingAnimationTrigger = "attacking";
    string dyingAnimationTrigger = "dying";
    string walkingAnimationBool = "walking";

    void Start()
    {
        firstPosition = GameObject.FindGameObjectWithTag("FirstPosition").GetComponent<Transform>().position;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        transform.position = firstPosition;
        playerSpeed = 1.5f;
        // this is used to insure having the same speed in all the directions
        playerLateralSpeed = playerSpeed / Mathf.Sqrt(2);
        isFacingRight = true;
        isAttacking = false;
    }

    void Update()
    {
        // stops mouvement if the attack animation is on
        if (GetAnimationState() == animationState.Attack)
        {
            isAttacking = false;
        }
        else
        {
            isAttacking = true;
        }

        Vector2 mouvementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (isAttacking && Vector2.zero != mouvementVector)
        {
            playerLateralSpeed = playerSpeed / Mathf.Sqrt(2);
            if ((mouvementVector.x > 0 && !isFacingRight) || (mouvementVector.x < 0 && isFacingRight))
            {
                Flip();
            }
            animator.SetBool(walkingAnimationBool, true);
            Move(mouvementVector);
        }
        else
        {
            animator.SetBool(walkingAnimationBool, false);
        }

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

    // uses the same animations, just flip the sprite
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    //to adjust the speed to the type of mouvement (lateral or horizontal/vertical)
    void Move(Vector2 mouvement_vector)
    {
        if (mouvement_vector.x * mouvement_vector.y == 0)
        {
            rb2d.MovePosition(rb2d.position + mouvement_vector * Time.deltaTime * playerSpeed);
        }
        else
        {
            rb2d.MovePosition(rb2d.position + mouvement_vector * Time.deltaTime * playerLateralSpeed);
        }
    }

    animationState GetAnimationState()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return animationState.Attack;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            return animationState.Walk;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            return animationState.Dead;
        }
        else
        {
            return animationState.Idle;
        }

    }

}
