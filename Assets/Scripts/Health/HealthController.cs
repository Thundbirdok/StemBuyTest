namespace Health
{
    using System;
    using Player;
    using Unity.Netcode;
    using UnityEngine;
    
    public class HealthController : NetworkBehaviour
    {
        public event Action OnHealthChanged;
        public event Action<PlayerCharacterController> OnDeath;
        
        public ushort Health => _healthVariable.Value;

        private readonly NetworkVariable<ushort> _healthVariable = new NetworkVariable<ushort>();
        
        [field: SerializeField]
        public ushort MaxHealth { get; private set; }

        private PlayerCharacterController _player;
        
        public void Initialize(PlayerCharacterController player) => _player = player;

        public void Enable()
        {
            _healthVariable.OnValueChanged += InvokeOnHealthChanged;
            
            _healthVariable.Value = MaxHealth;
        }

        public void Disable()
        {
            _healthVariable.OnValueChanged -= InvokeOnHealthChanged;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                return;
            }
            
            _healthVariable.Value = (ushort)Mathf.Max(Health - damage, 0);
        }

        private void InvokeOnHealthChanged(ushort _1, ushort _2)
        {
            OnHealthChanged?.Invoke();

            if (Health > 0)
            {
                return;
            }

            Debug.Log("Player died: " + _player.NetworkObject.OwnerClientId);
                
            OnDeath?.Invoke(_player);
        }
    }
}
