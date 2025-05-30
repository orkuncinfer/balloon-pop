using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Transporting;
using UnityEngine;

namespace ECM2.Examples.Networking.FishNet
{
    /// <summary>
    /// This example follows the OLD FishNet character controller prediction example (Now removed from FishNet?),
    /// and shows how to replace the built-in CharacterController with ECM2 CharacterMovement component.
    /// This shows the minimal data required to sync.
    /// </summary>
    
    public class PlayerController : NetworkBehaviour
    {
        #region STRUCTS

        /// <summary>
        /// Input movement data.
        /// </summary>
        
        public struct ReplicateData : IReplicateData
        {
            public float horizontal;
            public float vertical;
            public bool jump;
        
            private uint _tick;

            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }
        
        /// <summary>
        /// Reconciliation data.
        /// </summary>
        
        public struct ReconcileData : IReconcileData
        {
            public Vector3 position;
            public Quaternion rotation;
        
            public Vector3 velocity;
        
            public bool constrainedToGround;
            public float unconstrainedTime;
        
            public bool hitGround;
            public bool isWalkable;
        
            private uint _tick;
            
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        #endregion

        #region EDITOR EXPOSED FIELDS

        public float rotationRate = 540.0f;

        public float maxSpeed = 5;

        public float acceleration = 20.0f;
        public float deceleration = 20.0f;

        public float groundFriction = 8.0f;
        public float airFriction = 0.5f;

        public float jumpImpulse = 6.5f;

        [Range(0.0f, 1.0f)]
        public float airControl = 0.3f;

        public Vector3 gravity = Vector3.down * 9.81f;

        #endregion

        #region FIELDS

        private bool _jump;

        #endregion

        #region PROPERTIES

        private CharacterMovement characterMovement { get; set; }

        #endregion

        #region METHODS

        private ReplicateData CreateReplicateData()
        {
            if (!IsOwner)
                return default;

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            ReplicateData replicateData = new()
            {
                horizontal = horizontal,
                vertical = vertical,
                jump = _jump
            };

            _jump = false;

            return replicateData;
        }

        [Replicate]
        private void RunInputs(ReplicateData md, ReplicateState state = ReplicateState.Invalid,
            Channel channel = Channel.Unreliable)
        {
            // Jump
            
            if (md.jump && characterMovement.isGrounded)
            {
                characterMovement.PauseGroundConstraint();
                characterMovement.velocity.y = Mathf.Max(characterMovement.velocity.y, jumpImpulse);
            }
            
            // Movement

            Vector3 moveDirection = Vector3.right * md.horizontal + Vector3.forward * md.vertical;
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
            
            Vector3 desiredVelocity = moveDirection * maxSpeed;

            float actualAcceleration = characterMovement.isGrounded ? acceleration : acceleration * airControl;
            float actualDeceleration = characterMovement.isGrounded ? deceleration : 0.0f;

            float actualFriction = characterMovement.isGrounded ? groundFriction : airFriction;

            float deltaTime = (float)TimeManager.TickDelta;
            characterMovement.RotateTowards(moveDirection, rotationRate * deltaTime);
            characterMovement.SimpleMove(desiredVelocity, maxSpeed, actualAcceleration, actualDeceleration,
                actualFriction, actualFriction, gravity, true, deltaTime);
        }
        
        public override void CreateReconcile()
        {
            ReconcileData reconcileData = new ReconcileData
            {
                position = characterMovement.position,
                rotation = characterMovement.rotation,
                velocity = characterMovement.velocity,
                constrainedToGround = characterMovement.constrainToGround,
                unconstrainedTime = characterMovement.unconstrainedTimer,
                hitGround = characterMovement.currentGround.hitGround,
                isWalkable = characterMovement.currentGround.isWalkable
            };

            ReconcileState(reconcileData);
        }

        [Reconcile]
        private void ReconcileState(ReconcileData rd, Channel channel = Channel.Unreliable)
        {
            characterMovement.SetState(
                rd.position,
                rd.rotation,
                rd.velocity,
                rd.constrainedToGround,
                rd.unconstrainedTime,
                rd.hitGround,
                rd.isWalkable);
        }
        
        private void OnTick()
        {
            RunInputs(CreateReplicateData());
        }
        
        private void OnPostTick()
        {
            CreateReconcile();
        }
        
        public override void OnStartNetwork()
        {
            TimeManager.OnTick += OnTick;
            TimeManager.OnPostTick += OnPostTick;
        }
        
        public override void OnStopNetwork()
        {
            if (TimeManager == null)
                return;
            
            TimeManager.OnTick -= OnTick;
            TimeManager.OnPostTick -= OnPostTick;
        }

        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            characterMovement = GetComponent<CharacterMovement>();
        }
        
        private void Update()
        {
            if (!IsOwner)
                return;
            
            if (Input.GetKeyDown(KeyCode.Space))
                _jump = true;
        }

        #endregion
    }
}
