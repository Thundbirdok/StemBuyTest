namespace Health
{
    using System;
    using Unity.Netcode;
    using UnityEngine;

    [Serializable]
    public class HealthController
    {
        public event Action OnHealthChanged;
        public event Action OnDeath;
        
        public ushort Health => HealthVariable.Value;

        [SerializeField]
        private HealthNetworkVariableWrapper healthWrapper;
        
        [field: SerializeField]
        public ushort MaxHealth { get; private set; }

        private NetworkVariable<ushort> HealthVariable => healthWrapper.Health;
        
        public void Enable()
        {
            HealthVariable.OnValueChanged += InvokeOnHealthChanged;
            
            HealthVariable.Value = MaxHealth;
        }

        public void Disable()
        {
            HealthVariable.OnValueChanged -= InvokeOnHealthChanged;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                return;
            }
            
            HealthVariable.Value = (ushort)Mathf.Max(Health - damage, 0);

            if (Health == 0)
            {
                OnDeath?.Invoke();
            }
        }

        private void InvokeOnHealthChanged(ushort _1, ushort _2)
        {
            OnHealthChanged?.Invoke();
        }
    }
}
