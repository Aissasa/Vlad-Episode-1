using UnityEngine;
using System.Collections;
using System;

namespace PlayerLogic
{
    public class PlayerStateHandler : MonoBehaviour, IDamageable, ICombatEffect
    {
        // urgent : add look at to player and enemy when hit
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

        public BasicStats stats { get; protected set; }


        public Transform spawnPosition;
        public LayerMask damageableLayer;
        [Range(0.1f, 10f)]
        public float playerSpeed;
        // todo : may change depending on the weapon rather than here
        [Range(0.1f, 10f)]
        public float playerAttackRange;

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
            }
        }

        protected void DetectAttack()
        {
            // new implement attacking logic here, maybe launch an event , make a raycast and detect the enemies hit, 
            // and the sub is a damage handler, that makes calculations
            if (Input.GetButton("Attack"))
            {
                isAttacking = true;
            }

            if (Input.GetButtonUp("Attack"))
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
            movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        public void ShowCombatEffects()
        {
            // todo : attack outcome
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