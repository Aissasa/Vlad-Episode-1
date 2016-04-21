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

        public IEnemyState currentEnemyState { protected get; set; }
        public PatrolState patrolState { get; protected set; }
        public ChaseState chaseState { get; protected set; }
        public LookOutState lookOutState { get; protected set; }
        public AttackState attackState { get; protected set; }

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
        public string lookingOutAnimationTrigger
        {
            get { return "lookingOut"; }
        }
        public string walkingAnimationBool
        {
            get { return "walking"; }
        }

        public bool isFacingRight { get; set; }         // bool to know which direction the character is facing
        public SpriteRenderer spriteRenderer { get; protected set; }

        public BasicStats stats { get; protected set; }

        [Header("For Debugging Purposes: ")]
        public bool displayPath;           // display path gizmos or not
        [Space(10)]

        [Header("The unwalkable layer: ")]
        public LayerMask unwalkableLayer;
        [Space(10)]

        [Header("Key positions: ")]
        public Transform player;
        public Transform spawnPosition;       // the position of first spawn
        public Transform[] patrolWaypoints;
        [Space(10)]

        [Header("Speed and delays: ")]
        [Range(0.1f, 10f)]
        public float characterSpeed;       // speed for normal mouvement
        [Space(5)]
        [Range(0.1f, 10f)]
        public float patrolDelay;       // how much to wait in patrol point
        [Range(0.5f, 5)]
        public float lookOutDelay;          // look out wait time
        [Range(0.5f, 10f)]
        public float attackDelay;       // delay between attacks

        [HideInInspector]
        public float attackRange;
        [HideInInspector]
        public float chasingRange;
        [HideInInspector]
        public float pursuitRange;

        protected BasicStats.AttackOutcome outcome;
        protected bool isDead;

        protected void Awake()
        {
            Spawn();
            anim = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            patrolState = new PatrolState(this);
            chaseState = new ChaseState(this);
            lookOutState = new LookOutState(this);
            attackState = new AttackState(this);
            if (patrolWaypoints == null || patrolWaypoints.Length < 1)
            {
                patrolWaypoints = new Transform[] { spawnPosition };
                // note : maybe add another point by raycasting (walkable)
            }
            //else
            //{
            //    List<Transform> list = new List<Transform>(patrolWaypoints);
            //    list.Add(spawnPosition);
            //    patrolWaypoints = list.ToArray();
            //}
        }

        protected void Start()
        {
            currentEnemyState = patrolState;
            stats = BasicStats.EnemyTest();
            if (spriteRenderer.flipX)
                isFacingRight = false;
            else
                isFacingRight = true;
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
            isDead = false;
        }

        protected void FixedUpdate()
        {
            if (!InBlockingAnimation())
            {
                currentEnemyState.UpdateState();
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
                Debug.Log(gameObject.name +" is dead !");
                anim.SetTrigger(dyingAnimationTrigger);
                enabled = false;
                DeadEnemy(gameObject);
            }
            else
            {
                if (!InBlockingAnimation() && (outcome == BasicStats.AttackOutcome.Crit || outcome == BasicStats.AttackOutcome.Hit))
                {
                    anim.SetTrigger(hitAnimationTrigger);
                }
                else
                {
                    anim.ResetTrigger(hitAnimationTrigger);
                }
            }
        }

        protected void OnDrawGizmos()
        {
            if (currentEnemyState == null)
            {
                return;
            }
            if (displayPath)
            {
                currentEnemyState.DrawGizmos();
            }

            Gizmos.DrawSphere(spriteRenderer.bounds.center, 0.02f);

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
            if (damage > stats.currentHealth)
            {
                stats.currentHealth = 0;
            }
            else
            {
                stats.currentHealth  = stats.currentHealth - damage;
            }

            Debug.Log(outcome + " => Damage inflicted by player = " + damage);
            Debug.Log("Enemy health : " + stats.currentHealth + "/" + stats.maxHealth);
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
