using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

namespace PlayerLogic
{
    public class PlayerStateHandler : MonoBehaviour, IDamageable, ICombatEffect
    {
        public delegate void DeadPlayerAction(GameObject go);
        public static event DeadPlayerAction DeadPlayer;

        // urgent : add lookat to player and enemy when hit
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

        public Vector2 movementVector { get;  set; }
        public bool isAttacking { get; set; }

        public BasicStats stats { get; protected set; }


        public Transform spawnPosition;
        public LayerMask damageableLayer;
        [Range(0.1f, 10f)]
        public float playerSpeed;
        // todo : may change depending on the weapon rather than here
        [Range(0.1f, 10f)]
        public float playerAttackRange;

        // todo : add attack cooldown

        protected BasicStats.AttackOutcome outcome;
        protected bool isDead;
        

        protected void Awake()
        {
            Spawn();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            attackState = new AttackState(this);
            idleState = new IdleState(this);
            moveState = new MoveState(this);

            stats = BasicStats.PlayerTest();
        }

        protected void Start()
        {
            currentPlayerState = idleState;
            if (spriteRenderer.flipX)
                isFacingRight = false;
            else
                isFacingRight = true;
            isAttacking = false;
            isDead = false;
        }

        protected void Update()
        {
            UpdateMovementVector();
            DetectAttack();
            if (!InBlockingAnimation())
            {
                currentPlayerState.UpdateState();
            }
        }

        public void Flip()
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
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

        public bool InBlockingAnimation()
        {
            MyAnimationState animState = GetAnimationState();

            switch (animState)
            {
                case MyAnimationState.Attack:
                    return true;
                case MyAnimationState.Dead:
                    return true;
                case MyAnimationState.Hit:
                    return true;
                default:
                    return false;
            }
        }

        public void TakeDamage(BasicStats attackerStats)
        {
            if (isDead)
            {
                return;
            }
            int damage = DamageCalculationManager.Instance.CalculateInflictedDamage(attackerStats, stats, out outcome);
            UpdateHealth(damage);
            if (stats.currentHealth <= 0)
            {
                isDead = true;
                Debug.Log("Player is dead !");
                anim.SetTrigger(dyingAnimationTrigger);
                enabled = false;
                DeadPlayer(gameObject);
            }
        }

        protected void DetectAttack()
        {
            if (CrossPlatformInputManager.GetButton("Attack"))
            {
                isAttacking = true;
            }

            if (CrossPlatformInputManager.GetButtonUp("Attack"))
            {
                isAttacking = false;
            }
        }

        protected void Spawn()
        {
            if (spawnPosition != null)
            {
                transform.position = spawnPosition.position;
            }
        }

        protected void UpdateHealth(int damage)
        {
            if (damage>stats.currentHealth)
            {
                stats.currentHealth = 0;
            }
            else
            {
                stats.currentHealth -= damage;
            }
            Debug.Log(outcome + " => Damage inflicted by enemy = " + damage);
            Debug.Log("Player health : " + stats.currentHealth + "/" + stats.maxHealth);
        }

        protected void UpdateMovementVector()
        {
            movementVector = new Vector2(CrossPlatformInputManager.GetAxisRaw("Horizontal"), CrossPlatformInputManager.GetAxisRaw("Vertical"));
            // todo : make movement vect coordinates >0.5
            //anim.SetFloat("x", movementVector.x);
            //anim.SetFloat("y", movementVector.y);
        }

        public void ShowCombatEffects()
        {
            // todo : attack outcome floating texts
            Debug.Log(outcome);
        }

        public enum MyAnimationState
        {
            Idle,
            Walk,
            Attack,
            Hit,
            Dead
        }

        // ps : for debug
        //void OnDrawGizmos()
        //{
        //    Gizmos.DrawSphere(spriteRenderer.bounds.center, 0.02f);
        //}
    }

}