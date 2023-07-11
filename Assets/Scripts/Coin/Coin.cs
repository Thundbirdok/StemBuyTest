
namespace Coin
{
    using Player;
    using Unity.Netcode;
    using UnityEngine;

    public class Coin : NetworkBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out PlayerCharacterController player) == false)
            {
                return;
            }

            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }
            
            NetworkObject.Despawn();
            
            player.AddCoin();
        }
    }
}
