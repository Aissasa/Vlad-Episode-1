using UnityEngine;
using System.Collections;
using System;

namespace PlayerLogic
{
    public class PlayerStateHandler : MonoBehaviour
    {

        public IPlayerState currentPlayerState { get; set; }
        public AttackState attackState { get; protected set; }
        public IdleState idleState { get; protected set; }
        public MoveState moveState { get; protected set; }

        public Animator anim { get; protected set; }

        // animator's triggers and bools names
        public string attackingAnimationTrigger
        {
            get { return "attacking"; }
        }
        public string dyingAnimationTrigger
        {
            get { return "dying"; }
        }
        public string hitAnimationTrigger
        {
            get { return "hit"; }
        }
        public string walkingAnimationBool
        {
            get { return "walking"; }
        }

        public SpriteRenderer spriteRenderer { get; protected set; }
        public bool isFacingRight { get; set; }

        public Vector2 movementVector { get; protected set; }
        public bool isAttacking { get; set; }

        public Transform spawnPosition;
        [Range(0.1f, 10f)]
        public float playerSpeed;   

        protected void Awake()
        {
            Spawn();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackState = new AttackState(this);
            idleState = new IdleState(this);
            moveState = new MoveState(this);
        }

        protected void Start()
        {
            currentPlayerState = idleState;
            if (spriteRenderer.flipX)
                isFacingRight = false;
            else
                isFacingRight = true;
            isAttacking = false;
        }

        protected void Update()
        {
            UpdateMovementVector();
            DetectAttack();
            currentPlayerState.UpdateState();
        }

        public bool CanMove()
        {
            MyAnimationState animState = GetAnimationState();

            switch (animState)
            {
                case MyAnimationState.Attack:
                    return false;
                case MyAnimationState.Dead:
                    return false;
                case MyAnimationState.Hit:
                    return false;
                default:
                    return true;
            }
        }
    
        protected void DetectAttack()
        {
            // new implement attacking logic here, maybe launch an event , make a raycast and detect the enemies hit, and the sub is a damage handler, that makes calculations
            if (Input.GetButton("Attack"))
            {
                isAttacking = true;
            }

            if (Input.GetButtonUp("Attack"))
            {
                isAttacking = false;
            }
        }

        public void Flip()
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        protected void Spawn()
        {
            if (spawnPosition != null)
            {
                transform.position = spawnPosition.position;
            }
        }

        protected void UpdateMovementVector()
        {
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        public enum MyAnimationState
        {
            Idle,
            Walk,
            Attack,
            Hit,
            Dead
        }

        public MyAnimationState GetAnimationState()
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                return MyAnimationState.Attack;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                return MyAnimationState.Walk;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                return MyAnimationState.Dead;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            {
                return MyAnimationState.Dead;
            }
            else
            {
                return MyAnimationState.Idle;
            }
        }
    }

}