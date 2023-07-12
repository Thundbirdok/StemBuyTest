namespace Projectile
{
    using Health;
    using Unity.Netcode;
    using UnityEngine;

    public class Projectile : NetworkBehaviour
    {
        [SerializeField]
        private int damage = 20;
        
        [SerializeField]
        private float speed = 10;
        
        [SerializeField]
        private Rigidbody2D body;

        private Vector2 _direction;

        private bool _isDestroyed;
        
        private void FixedUpdate()
        {
            var distance = speed * Time.fixedDeltaTime;
            var delta = _direction * distance;
            var newPosition = body.position + delta;
            
            body.MovePosition(newPosition);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (IsServer == false)
            {
                return;
            }
            
            if (_isDestroyed)
            {
                return;
            }
            
            if (other.TryGetComponent(out ITakeDamage takeDamage))
            {
                takeDamage.TakeDamage(damage);
            }

            if (other.TryGetComponent(out IStopProjectile _))
            {
                NetworkObject.Despawn();

                _isDestroyed = true;
            }
        }

        public void Initialize(Vector2 direction) 
        {
            _direction = direction;

            _isDestroyed = false;
        }
    }
}
