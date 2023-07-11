namespace Health
{
    using System;
    using UnityEngine;

    [Serializable]
    public class HealthSpriteRendererView
    {
        [SerializeField]
        private Transform healthBarHandler;
        
        [SerializeField]
        private Transform healthSlider;
        
        private HealthController _healthController;

        public void Initialize(HealthController healthController)
        {
            _healthController = healthController;
        }

        public void Enable()
        {
            _healthController.OnHealthChanged += SetHealthBar;
            
            SetHealthBar();
        }

        public void Disable()
        {
            _healthController.OnHealthChanged -= SetHealthBar;
        }

        public void UpdatePosition(float ownerRotation)
        {
            healthBarHandler.rotation = Quaternion.Euler(0, 0, -ownerRotation);
        }
        
        private void SetHealthBar()
        {
            healthSlider.localScale = new Vector3
            (
                _healthController.Health / (float)_healthController.MaxHealth,
                1,
                1
            );
        }
    }
}
