using UnityEngine;

namespace Coin
{
    using Player;
    using TMPro;

    public class CoinsView : MonoBehaviour
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
            
            PlayerCharacterController.Singleton.OnCoinsChanged += SetCoinsText;
            SetCoinsText();
        }

        private void Disable()
        {
            PlayerCharacterController.Singleton.OnCoinsChanged -= SetCoinsText;
        }
        
        private void SetCoinsText()
        {
            text.text = PlayerCharacterController.Singleton.Coins.ToString();
        }
    }
}
