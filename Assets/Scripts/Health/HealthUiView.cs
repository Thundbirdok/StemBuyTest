using UnityEngine;

namespace Health
{
    using Player;
    using TMPro;

    public class HealthUiView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;
        
        private void OnEnable()
        {
            if (PlayerCharacterController.Singleton == null)
            {
                PlayerCharacterController.OnPlayerSpawned += Enable;
                
                return;
            }

            Enable();
        }

        private void OnDisable() => Disable();

        private void Enable()
        {
            PlayerCharacterController.OnPlayerSpawned -= Enable;
            
            PlayerCharacterController.Singleton.HealthController.OnHealthChanged += SetHealthText;
            SetHealthText();
        }

        private void Disable()
        {
            if (PlayerCharacterController.Singleton == null)
            {
                return;
            }
            
            PlayerCharacterController.Singleton.HealthController.OnHealthChanged -= SetHealthText;
        }
        
        private void SetHealthText()
        {
            text.text = PlayerCharacterController.Singleton.HealthController.Health.ToString();
        }
    }
}
