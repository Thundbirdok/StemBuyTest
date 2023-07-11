namespace Projectile
{
    using Player;
    using Unity.Netcode;
    using UnityEngine;

    public class Projectile : NetworkBehaviour
    {
        public NetworkBehaviour NetworkOwner { get; private set; }
        
        [SerializeField]
        private int damage = 20;
        
        [SerializeField]
        private float speed = 10;
        
        [SerializeField]
        private Rigidbody2D body;

        private Vector2 _direction;
        
        private void FixedUpdate()
        {
            var distance = speed * Time.fixedDeltaTime;
            var delta = _direction * distance;
            var newPosition = body.position + delta;
            
            body.MovePosition(newPosition);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.TryGetComponent(out PlayerCharacterController player))
            {
                player.TakeDamage(damage);
            }

            if (other.collider.TryGetComponent(out IStopProjectile _))
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(NetworkBehaviour networkOwner, Vector2 direction)
        {
            NetworkOwner = networkOwner;
            _direction = direction;
        }
    }
}
