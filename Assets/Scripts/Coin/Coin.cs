namespace Coin
{
    using Player;
    using Unity.Netcode;
    using UnityEngine;

    public class Coin : NetworkBehaviour
    {
        [SerializeField]
        private ushort coinValue = 1;

        private bool _isDestroyed;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _isDestroyed = false;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isDestroyed)
            {
                return;
            }
            
            if (other.TryGetComponent(out PlayerCharacterController player) == false)
            {
                return;
            }

            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }

            _isDestroyed = true;
            
            NetworkObject.Despawn();
            
            player.AddCoin(coinValue);
        }
    }
}
