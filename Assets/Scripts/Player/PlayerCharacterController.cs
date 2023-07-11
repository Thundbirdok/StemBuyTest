namespace Player
{
    using System;
    using Health;
    using Projectile;
    using Unity.Netcode;
    using UnityEngine;

    public class PlayerCharacterController : NetworkBehaviour, ITakeDamage, IStopProjectile
    {
        public static event Action OnPlayerSpawned;
        public event Action OnCoinsChanged;

        public static PlayerCharacterController Singleton;
        
        private ushort _coins;
        public ushort Coins
        {
            get
            {
                return _coins;
            }

            private set
            {
                if (_coins == value)
                {
                    return;
                }

                _coins = value;
                
                OnCoinsChanged?.Invoke();
            }
        }
        
        [SerializeField]
        private PlayerMoveController moveController;

        [SerializeField]
        private FireController fireController;
        
        [field: SerializeField]
        public HealthController HealthController { get; private set; }

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
            HealthController.TakeDamage(damage);
        }
        
        private void UpdateMoveInput() => moveController.UpdateInput(_inputHandler.MoveInput);
        
        private void UpdateFireInput() => fireController.UpdateFireInput(_inputHandler.IsFireInput);

        public void AddCoin()
        {
            Coins += 1;
        }
    }
}
