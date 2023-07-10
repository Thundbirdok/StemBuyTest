namespace Player
{
    using Unity.Netcode;
    using UnityEngine;

    public class PlayerCharacterController : NetworkBehaviour
    {
        [SerializeField]
        private PlayerMoveController moveController;

        [SerializeField]
        private SpriteRenderer body;
        
        [SerializeField]
        private Color ownedColor;
        
        [SerializeField]
        private Color unownedColor;
        
        private InputHandler _inputHandler;
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            body.color = IsOwner ? ownedColor : unownedColor;
            
            if (IsOwner == false)
            {
                return;
            }
            
            _inputHandler = new InputHandler();
            
            _inputHandler.Enable();
            
            _inputHandler.OnInputChanged += UpdateInput;
            
            moveController.UpdateInput(_inputHandler.Input);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (IsOwner  == false)
            {
                return;
            }

            _inputHandler.OnInputChanged -= UpdateInput;
            
            _inputHandler.Disable();
        }

        private void FixedUpdate() => moveController.UpdatePosition(Time.fixedDeltaTime);

        private void UpdateInput() => moveController.UpdateInput(_inputHandler.Input);
    }
}
