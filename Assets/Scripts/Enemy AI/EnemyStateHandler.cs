using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace EnemyAI
{
    public class EnemyStateHandler : MonoBehaviour, IDamageable
    {
        public delegate void DeadEnemyAction(GameObject go);
        public static event DeadEnemyAction DeadEnemy;

        public delegate void HitEnemyAction();
        public static event HitEnemyAction HitEnemy;

        public delegate void EnemyDamagedAction(GameObject enemy, int damage, BasicStats.AttackOutcome outcome);
        public static event EnemyDamagedAction DamagedEnemy;


        public IEnemyState CurrentEnemyState { protected get; set; }
        public PatrolState PatrolState { get; protected set; }
        public ChaseState ChaseState { get; protected set; }
        public LookOutState LookOutState { get; protected set; }
        public AttackState AttackState { get; protected set; }

        public Animator Anim { get; protected set; }

        // animator's triggers and bools names
        public string AttackingAnimationTrigger
        {
            get { return "attacking"; }
        }
        public string DyingAnimationTrigger
        {
            get { return "dying"; }
        }
        public string HitAnimationTrigger
        {
            get { return "hit"; }
        }
        public string LookingOutAnimationTrigger
        {
            get { return "lookingOut"; }
        }
        public string WalkingAnimationBool
        {
            get { return "walking"; }
        }

        public bool IsFacingRight { get; set; }         // bool to know which direction the character is facing
        public SpriteRenderer SpriteRend { get; protected set; }

        public BasicStats EnemyStats { get; protected set; }

        [Header("For Debugging Purposes: ")]
        [SerializeField]
        private bool displayPath;           // display path gizmos or not


        [Space(10)]
        [Header("The unwalkable layer: ")]
        [SerializeField]
        private LayerMask unwalkableLayer;
        public LayerMask UnwalkableLayer { get { return unwalkableLayer; } }


        [Space(10)]
        [Header("Key positions: ")]
        [SerializeField]
        private Transform player;
        public Transform Player { get { return player; } }

        [SerializeField]
        private Transform spawnPosition;     

        [SerializeField]
        private Transform[] patrolWayPoints;
        public Transform[] PatrolWayPoints { get { return patrolWayPoints; } }


        [Space(10)]
        [Header("Speed and delays: ")]
        [Range(0.1f, 10f)]
        [SerializeField]
        private float characterSpeed;  
        public float CharacterSpeed { get { return characterSpeed; } }


        [Space(5)]
        [Range(0.1f, 10f)]
        [SerializeField]
        private float patrolDelay;       // how much to wait in patrol point
        public float PatrolDelay { get { return patrolDelay; } }

        [Range(0.5f, 5)]
        [SerializeField]
        private float lookOutDelay;          // look out wait time
        public float LookOutDelay { get { return lookOutDelay; } }

        [Range(0.5f, 10f)]
        [SerializeField]
        private float attackDelay;       // delay between attacks
        public float AttackDelay { get { return attackDelay; } }

        [HideInInspector]
        public float attackRange;
        [HideInInspector]
        public float attackReach;
        [HideInInspector]
        public float chasingRange;
        [HideInInspector]
        public float pursuitRange;

        protected BasicStats.AttackOutcome outcome;
        protected bool isDead;

        protected void Awake()
        {
            Spawn();
            Anim = GetComponent<Animator>();
            SpriteRend = GetComponent<SpriteRenderer>();
            PatrolState = new PatrolState(this);
            ChaseState = new ChaseState(this);
            LookOutState = new LookOutState(this);
            AttackState = new AttackState(this);
            if (patrolWayPoints == null || patrolWayPoints.Length < 1)
            {
                patrolWayPoints = new Transform[] { spawnPosition };
                // note : maybe add another point by raycasting (walkable)
            }
        }

        protected void Start()
        {
            CurrentEnemyState = PatrolState;
            EnemyStats = BasicStats.EnemyTest();
            if (SpriteRend.flipX)
                IsFacingRight = false;
            else
                IsFacingRight = true;
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            isDead = false;
        }

        protected void FixedUpdate()
        {
            if (GameManager.Instance.PlayerIsDead)
            {
                CurrentEnemyState = PatrolState;
            }
            if (!InBlockingAnimation())
            {
                CurrentEnemyState.UpdateState();
            }
        }

        public void Flip()
        {
            IsFacingRight = !IsFacingRight;
            SpriteRend.flipX = !SpriteRend.flipX;
        }

        public MyAnimationState GetAnimationState()
        {
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                return MyAnimationState.Attack;
            }
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                return MyAnimationState.Walk;
            }
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
            {
                return MyAnimationState.Dead;
            }
            if (Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
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
                //case MyAnimationState.Hit:
                    //return true;
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
            int damage = DamageCalculationManager.Instance.CalculateInflictedDamage(attackerStats, EnemyStats, out outcome);
            UpdateHealth(damage);
            if (DamagedEnemy != null)
            {
                DamagedEnemy(gameObject, damage, outcome);
            }
            if (HitEnemy != null && (outcome == BasicStats.AttackOutcome.Hit || outcome == BasicStats.AttackOutcome.Crit))
            {
                HitEnemy();
            }
            if (EnemyStats.CurrentHealth <= 0)
            {
                isDead = true;
                Anim.SetTrigger(DyingAnimationTrigger);
                enabled = false;
                if (DeadEnemy!= null)
                {
                    DeadEnemy(gameObject);
                }
            }
            else
            {
                if (!InBlockingAnimation() && (outcome == BasicStats.AttackOutcome.Crit || outcome == BasicStats.AttackOutcome.Hit))
                {
                    Anim.SetTrigger(HitAnimationTrigger);
                }
                else
                {
                    Anim.ResetTrigger(HitAnimationTrigger);
                }
            }
        }

        protected void DamagePlayer()
        {
            if (PlayerInAttackReach())
            {
                player.GetComponent<IDamageable>().TakeDamage(EnemyStats);
            }
        }

        protected void OnDrawGizmos()
        {
            if (CurrentEnemyState == null)
            {
                return;
            }
            if (displayPath)
            {
                CurrentEnemyState.DrawGizmos();
            }

        }

        protected bool PlayerInAttackReach()
        {
            return DirectionAndDistanceCalculator.CalculateDistance(transform.Get2DPosition(), player.Get2DPosition()) < attackReach;
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
            if (damage > EnemyStats.CurrentHealth)
            {
                EnemyStats.CurrentHealth = 0;
            }
            else
            {
                EnemyStats.CurrentHealth  = EnemyStats.CurrentHealth - damage;
            }
        }

        public enum MyAnimationState
        {
            Idle,
            Walk,
            Attack,
            Hit,
            Dead
        }
          
    }
}
