namespace Player
{
    using Health;
    using Projectile;
    using Unity.Netcode;
    using UnityEngine;

    public class PlayerCharacterController : NetworkBehaviour, ITakeDamage, IStopProjectile
    {
        public ushort Coins { get; private set; }
        
        [SerializeField]
        private PlayerMoveController moveController;

        [SerializeField]
        private FireController fireController;
        
        [SerializeField]
        private HealthController healthController;

        [SerializeField]
        private HealthSpriteRendererView healthView;
        
        [SerializeField]
        private CharacterColorController colorController;
        
        private InputHandler _inputHandler;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            colorController.Paint(IsOwner);
            healthController.Enable();
            
            healthView.Initialize(healthController);
            healthView.Enable();
            
            if (IsOwner == false)
            {
                return;
            }
            
            _inputHandler = new InputHandler();
            
            _inputHandler.Enable();
            
            _inputHandler.OnMoveInputChanged += UpdateMoveInput;
            _inputHandler.OnFireInputChanged += UpdateFireInput;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            healthController.Disable();
            healthView.Disable();
            
            if (IsOwner  == false)
            {
                return;
            }

            _inputHandler.OnMoveInputChanged -= UpdateMoveInput;
            
            _inputHandler.Disable();
        }

        private void Update() => healthView.UpdatePosition(transform.rotation.z);

        private void FixedUpdate()
        {
            moveController.UpdatePosition(Time.fixedDeltaTime);
            fireController.UpdateFire(Time.fixedDeltaTime, transform.up);
        }

        public void TakeDamage(int damage)
        {
            healthController.TakeDamage(damage);
        }
        
        private void UpdateMoveInput() => moveController.UpdateInput(_inputHandler.MoveInput);
        
        private void UpdateFireInput() => fireController.UpdateFireInput(_inputHandler.IsFireInput);

        public void AddCoin()
        {
            Coins += 1;
        }
    }
}
