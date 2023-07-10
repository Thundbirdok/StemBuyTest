namespace Player
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PlayerMoveController
    {
        [SerializeField]
        private Rigidbody2D playerRigidbody;

        [SerializeField]
        private float speed;

        private Vector2 _input;

        public void UpdatePosition(float time)
        {
            var distance = speed * time;
            var direction = _input * distance;
            var newPosition = playerRigidbody.position + direction;

            playerRigidbody.MovePosition(newPosition);
        }

        public void UpdateInput(Vector2 input) => _input = input;
    }
}
