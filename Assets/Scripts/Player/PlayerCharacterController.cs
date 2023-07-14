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
        public static event Action OnControlledPlayerSpawned;

        public static event Action<PlayerCharacterController> OnPlayerSpawned;
        
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
            
            HealthController.Initialize(this);
            HealthController.Enable();
            
            CoinsHandler.Enable();

            HealthController.OnDeath += OnDeath;
            
            healthView.Initialize(HealthController);
            healthView.Enable();

            OnPlayerSpawned?.Invoke(this);
            
            if (IsOwner == false)
            {
                return;
            }

            Singleton = this;   
            OnControlledPlayerSpawned?.Invoke();
            
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
            _inputHandler.OnFireInputChanged -= UpdateFireInput;
            
            _inputHandler.Disable();
        }

        private void Update() => healthView.UpdatePosition(transform.rotation.z);

        private void FixedUpdate()
        {
            if (IsServer == false)
            {
                return;
            }
            
            moveController.UpdatePosition(Time.fixedDeltaTime);
            fireController.UpdateFire(Time.fixedDeltaTime, transform.up);
        }

        public void TakeDamage(int damage)
        {
            HealthController.TakeDamage(damage);
        }
        
        private void UpdateMoveInput() => UpdateMoveInputServerRpc(_inputHandler.MoveInput);
        
        [ServerRpc]
        private void UpdateMoveInputServerRpc(Vector2 input) => moveController.UpdateInput(input);
        
        private void UpdateFireInput() => UpdateFireInputServerRpc(_inputHandler.IsFireInput);

        [ServerRpc]
        private void UpdateFireInputServerRpc(bool isFireInput) => fireController.UpdateFireInput(isFireInput);
        
        public void AddCoin(ushort amount) => CoinsHandler.Add(amount);

        private void OnDeath(PlayerCharacterController player)
        {
            if (IsOwner == false)
            {
                return;
            }

            _inputHandler.OnMoveInputChanged -= UpdateMoveInput;
            _inputHandler.OnFireInputChanged -= UpdateFireInput;
            
            _inputHandler.Disable();
        }
    }
}
