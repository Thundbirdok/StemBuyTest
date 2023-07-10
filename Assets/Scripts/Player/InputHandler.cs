namespace Player
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class InputHandler
    {
        public event Action OnInputChanged;

        private Vector2 _input;
        public Vector2 Input
        {
            get
            {
                return _input;
            }

            private set
            {
                if (_input == value)
                {
                    return;
                }
                
                _input = value;
                
                OnInputChanged?.Invoke();
            }
        }
        
        private PlayerCharacterInputs.CharacterActions _actions;

        private bool _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            
            var playerCharacterInputs = new PlayerCharacterInputs();
            _actions = playerCharacterInputs.Character;
        }
        
        public void Enable()
        {
            Initialize();
            
            _actions.Enable();
            
            Subscribe();
        }

        public void Disable()
        {
            if (_isInitialized == false)
            {
                return;
            }

            _actions.Disable();
            
            Unsubscribe();
        }

        private void Subscribe()
        {
            _actions.Move.started += MoveInput;
            _actions.Move.performed += MoveInput;
            _actions.Move.canceled += MoveInput;
        }

        private void Unsubscribe()
        {
            _actions.Move.started -= MoveInput;
            _actions.Move.performed -= MoveInput;
            _actions.Move.canceled -= MoveInput;
        }

        private void MoveInput(InputAction.CallbackContext context)
        {
            Input = context.ReadValue<Vector2>();
        }
    }
}
