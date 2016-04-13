using UnityEngine;
using System.Collections;
using System;

namespace EnemyAI
{
    public class EnemyStateHandler : MonoBehaviour
    {
        public IEnemyState currentEnemyState { protected get; set; }
        public PatrolState patrolState { get; protected set; }
        public ChaseState chaseState { get; protected set; }
        public LookOutState lookOutState { get; protected set; }
        public AttackState attackState { get; protected set; }

        public Animator animator { get; protected set; }

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

        protected void Awake()
        {
            Spawn();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            patrolState = new PatrolState(this);
            chaseState = new ChaseState(this);
            lookOutState = new LookOutState(this);
            attackState = new AttackState(this);
            // todo : add this : use list
            //if (patrolWaypoints == null || patrolWaypoints.Length<1)
            //{
            //    patrolWaypoints = new Transform[] { spawnPosition };
            //}
            //else
            //{
            //    patrolWaypoints = new Transform[] { spawnPosition, patrolWaypoints };
            //}
        }

        protected void Start()
        {
            currentEnemyState = patrolState;
            if (spriteRenderer.flipX)
                isFacingRight = false;
            else
                isFacingRight = true;
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
            }
        }

        protected void FixedUpdate()
        {
            currentEnemyState.UpdateState();
            // new add hit here since it can happen anytime,
            // new add killed/died state for enemy, or actually add it here, by controlling health here
        }

        public void Flip()
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        public MyAnimationState GetAnimationState()
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
        }

        protected void Spawn()
        {
            if (spawnPosition != null)
            {
                transform.position = spawnPosition.position;
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
