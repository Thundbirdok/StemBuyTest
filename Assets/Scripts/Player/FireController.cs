namespace Player
{
    using System;
    using Projectile;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class FireController
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

            var projectile = Object.Instantiate(projectilePrefab);

            projectile.transform.position = firePosition.position;
            Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());
                
            projectile.Initialize(direction);

            _time = timeBetweenShots;
        }
    }
}
