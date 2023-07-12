namespace Player
{
    using System;
    using Coin;
    using Health;
    using Player.Fire;
    using Projectile;
    using Unity.Netcode;
    using UnityEngine;

    [RequireComponent
    (
        typeof(FireController),
        typeof(HealthController),
        typeof(CoinsHandler)
    )]
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
        public CoinsHandler CoinsHandler { get; private set; }

        [SerializeField]
        private HealthSpriteRendererView healthView;
        
        [SerializeField]
        private CharacterColorController colorController;
        
        private InputHandler _inputHandler;

        private void Reset()
        {
            fireController = GetComponent<FireController>();
            HealthController = GetComponent<HealthController>();
            CoinsHandler = GetComponent<CoinsHandler>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            colorController.Paint(IsOwner);
            HealthController.Enable();
            CoinsHandler.Enable();

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
            CoinsHandler.Disable();
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

        public void AddCoin(ushort amount) => CoinsHandler.Add(amount);

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
