namespace Player
{
    using System;
    using Unity.Netcode;
    using UnityEngine;

    [Serializable]
    public class HealthController
    {
        [SerializeField]
        private ushort maxHealth;

        [SerializeField]
        private Transform healthBar;

        private NetworkVariable<ushort> _health;

        public void Enable()
        {
            _health = new NetworkVariable<ushort>(maxHealth);
            SetHealthBar(0, maxHealth);
            
            _health.OnValueChanged += SetHealthBar;
        }

        public void Disable()
        {
            _health.OnValueChanged -= SetHealthBar;
        }

        private void SetHealthBar(ushort previousValue, ushort newValue)
        {
            healthBar.localScale = new Vector3(newValue / (float)maxHealth, 1, 1);
        }
    }
}
