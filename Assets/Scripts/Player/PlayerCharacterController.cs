namespace Player
{
    using System;
    using Coin;
    using Health;
    using Projectile;
    using Unity.Netcode;
    using UnityEngine;

    public class PlayerCharacterController : NetworkBehaviour, ITakeDamage, IStopProjectile
    {
        public static event Action OnPlayerSpawned;

        public static PlayerCharacterController Singleton;

        [SerializeField]
        private PlayerMoveController moveController;

        [SerializeField]
        private FireController fireController;

        [field: SerializeField]
        public HealthController HealthController { get; private set; }

        [field: SerializeField]
        public CoinsController CoinsController { get; private set; }

        [SerializeField]
        private HealthSpriteRendererView healthView;
        
        [SerializeField]
        private CharacterColorController colorController;
        
        private InputHandler _inputHandler;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            colorController.Paint(IsOwner);
            HealthController.Enable();
            CoinsController.Enable();

            HealthController.OnDeath += OnDeath;
            
            healthView.Initialize(HealthController);
            healthView.Enable();

            if (IsOwner == false)
            {
                return;
            }

            Singleton = this;   
            OnPlayerSpawned?.Invoke();
            
            _inputHandler = new InputHandler();
            
            _inputHandler.Enable();
            
            _inputHandler.OnMoveInputChanged += UpdateMoveInput;
            _inputHandler.OnFireInputChanged += UpdateFireInput;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            HealthController.Disable();
            CoinsController.Disable();
            healthView.Disable();
            
            HealthController.OnDeath -= OnDeath;
            
            if (IsOwner == false)
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
            HealthController.TakeDamage(damage);
        }
        
        private void UpdateMoveInput() => moveController.UpdateInput(_inputHandler.MoveInput);
        
        private void UpdateFireInput() => fireController.UpdateFireInput(_inputHandler.IsFireInput);

        public void AddCoin(ushort amount) => CoinsController.Add(amount);

        private void OnDeath()
        {
            if (IsServer == false)
            {
                return;
            }
            
            NetworkObject.Despawn();
        }
    }
}
