namespace Player.Fire
{
    using Network;
    using Projectile;
    using Unity.Netcode;
    using UnityEngine;

    public class FireController : NetworkBehaviour
    {
        [SerializeField]
        private Collider2D playerCollider;

        [SerializeField]
        private Transform firePosition;
        
        [SerializeField]
        private Projectile projectilePrefab;
        
        [SerializeField]
        private float timeBetweenShots = 0.25f;
        
        private bool _isFire;

        private float _time;
        
        public void UpdateFireInput(bool isFire) => _isFire = isFire;

        public void UpdateFire(float time, Vector2 direction)
        {
            _time -= time;

            if (_time > 0)
            {
                return;
            }
            
            _time = 0;

            if (_isFire == false)
            {
                return;
            }

            InstantiateProjectileServerRpc(direction);

            _time = timeBetweenShots;
        }
        
        [ServerRpc]
        private void InstantiateProjectileServerRpc(Vector2 direction)
        {
            var networkObject = NetworkObjectPool.Singleton.GetNetworkObject
            (
                projectilePrefab.gameObject,
                firePosition.position,
                Quaternion.identity
            );
            
            networkObject.Spawn(true);
            
            Physics2D.IgnoreCollision(playerCollider, networkObject.GetComponent<Collider2D>());

            networkObject.GetComponent<Projectile>().Initialize(direction);
        }
    }
}
