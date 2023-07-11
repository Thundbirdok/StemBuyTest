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
            Move(time);
            Rotate();
        }

        public void UpdateInput(Vector2 input) => _input = input;

        private void Rotate()
        {
            if (_input == Vector2.zero)
            {
                return;
            }

            var zRotation = (Mathf.Atan2(-_input.x, _input.y) * Mathf.Rad2Deg);
            playerRigidbody.transform.rotation = Quaternion.Euler(0, 0, zRotation);
        }

        private void Move(float time)
        {
            var distance = speed * time;
            var delta = _input * distance;
            var newPosition = playerRigidbody.position + delta;

            playerRigidbody.MovePosition(newPosition);
        }
    }
}
