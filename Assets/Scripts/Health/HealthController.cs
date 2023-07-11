namespace Health
{
    using System;
    using Unity.Netcode;
    using UnityEngine;

    [Serializable]
    public class HealthController
    {
        public event Action OnHealthChanged;
        
        public ushort Health => _health.Value;
        
        [field: SerializeField]
        public ushort MaxHealth { get; private set; }

        private NetworkVariable<ushort> _health;

        public void Enable()
        {
            _health = new NetworkVariable<ushort>(MaxHealth);

            _health.OnValueChanged += InvokeOnHealthChanged;
        }

        public void Disable()
        {
            _health.OnValueChanged -= InvokeOnHealthChanged;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                return;
            }
            
            _health.Value = (ushort)Mathf.Max(_health.Value - damage, 0);
        }

        private void InvokeOnHealthChanged(ushort _1, ushort _2)
        {
            OnHealthChanged?.Invoke();
        }
    }
}
