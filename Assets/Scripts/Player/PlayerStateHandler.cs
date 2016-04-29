using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.CrossPlatformInput;

namespace PlayerLogic
{
    public class PlayerStateHandler : MonoBehaviour, IDamageable
    {
        public delegate void DeadPlayerAction(GameObject go);
        public static event DeadPlayerAction DeadPlayer;

        public delegate void PlayerHitAction(Health health);
        public static event PlayerHitAction HitPlayer;

        public delegate void PlayerDamagedAction(GameObject player, int damage, BasicStats.AttackOutcome outcome);
        public static event PlayerDamagedAction DamagedPlayer;


        // urgent : add lookat to player and enemy when hit
        public IPlayerState CurrentPlayerState { get; set; }
        public AttackState AttackState { get; protected set; }
        public IdleState IdleState { get; protected set; }
        public MoveState MoveState { get; protected set; }

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
        public string WalkingAnimationBool
        {
            get { return "walking"; }
        }

        public SpriteRenderer SpriteRend { get; protected set; }
        public bool IsFacingRight { get; set; }

        public Vector2 MovementVector { get;  set; }
        public bool IsAttacking { get; set; }

        public BasicStats PlayerStats { get; protected set; }

        [SerializeField]
        private Transform playerSpawnPosition;
        private Transform PlayerSpawnPosition { get { return playerSpawnPosition; } }

        [SerializeField]
        private LayerMask damageableLayer;
        public LayerMask DamageableLayer { get { return damageableLayer; } }

        [Range(0.1f, 10f)]
        [SerializeField]
        private float playerSpeed;
        public float PlayerSpeed { get { return playerSpeed; } }

        // todo : may change depending on the weapon rather than here
        [Range(0.1f, 10f)]
        [SerializeField]
        private float playerAttackRange;
        public float PlayerAttackRange { get { return playerAttackRange; } }

        // todo : add attack cooldown

        protected BasicStats.AttackOutcome outcome;
        protected bool isDead;

        private Vector2 positionToRollTo;
        private float timer;

        protected void Awake()
        {
            Spawn();
            Anim = GetComponent<Animator>();
            SpriteRend = GetComponent<SpriteRenderer>();
            AttackState = new AttackState(this);
            IdleState = new IdleState(this);
            MoveState = new MoveState(this);

            PlayerStats = BasicStats.PlayerTest();
        }

        protected void Start()
        {
            CurrentPlayerState = IdleState;
            if (SpriteRend.flipX)
                IsFacingRight = false;
            else
                IsFacingRight = true;
            IsAttacking = false;
            isDead = false;
        }

        protected void Update()
        {
            // urgent : fix roll : add it as a state
            if (GameManager.Instance.GameTimer - timer < 0.5f)
            {
                LinearMouvement.Instance.MoveTo(gameObject, positionToRollTo, playerSpeed * 1.5f);
                return;
            }

            UpdateMovementVector();
            DetectAttack();
            if (!InBlockingAnimation())
            {
                CurrentPlayerState.UpdateState();
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
            int damage = DamageCalculationManager.Instance.CalculateInflictedDamage(attackerStats, PlayerStats, out outcome);
            UpdateHealth(damage);
            DamagedPlayer(gameObject, damage, outcome);
            HitPlayer(new Health(PlayerStats));
            if (PlayerStats.CurrentHealth <= 0)
            {
                isDead = true;
                Debug.Log("Player is dead !");
                Anim.SetTrigger(DyingAnimationTrigger);
                enabled = false;
                DeadPlayer(gameObject);
            }
        }

        protected void ApplyDamage()
        {
            if (CurrentPlayerState != AttackState)
            {
                Debug.Log("Not in attack state");
                return;
            }

            foreach (var character in AttackState.AttackTargets)
            {
                character.GetComponent<IDamageable>().TakeDamage(PlayerStats);
            }
        }

        protected void DetectAttack()
        {
            if (CrossPlatformInputManager.GetButton("Attack"))
            {
                IsAttacking = true;
            }

            if (CrossPlatformInputManager.GetButtonUp("Attack"))
            {
                IsAttacking = false;
            }
        }

        protected void Spawn()
        {
            if (PlayerSpawnPosition != null)
            {
                transform.position = PlayerSpawnPosition.position;
            }
        }

        protected void UpdateHealth(int damage)
        {
            if (damage>PlayerStats.CurrentHealth)
            {
                PlayerStats.CurrentHealth = 0;
            }
            else
            {
                PlayerStats.CurrentHealth -= damage;
            }
        }

        protected void UpdateMovementVector()
        {
            MovementVector = new Vector2(CrossPlatformInputManager.GetAxisRaw("Horizontal"), CrossPlatformInputManager.GetAxisRaw("Vertical"));
            // todo : make movement vect coordinates >0.5
            //Anim.SetFloat("x", MovementVector.x);
            //Anim.SetFloat("y", MovementVector.y);
        }

        void OnEnable()
        {
            Joystick.Roll += RollAction;
        }

        void OnDisable()
        {
            Joystick.Roll -= RollAction;
        }

        protected void RollAction(Vector2 direction)
        {
            // urgent : fix roll here too , add roll speed multip and roll time in editor
            timer = GameManager.Instance.GameTimer;
            positionToRollTo = transform.Get2DPosition() + direction;
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