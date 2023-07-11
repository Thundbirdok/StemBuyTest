namespace Player
{
    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    [Serializable]
    public class InputHandler
    {
        public event Action OnMoveInputChanged;
        public event Action OnFireInputChanged;
        
        private Vector2 _moveInput;

        public Vector2 MoveInput
        {
            get
            {
                return _moveInput;
            }

            private set
            {
                if (_moveInput == value)
                {
                    return;
                }
                
                _moveInput = value;
                
                OnMoveInputChanged?.Invoke();
            }
        }

        private bool _isFireInput;
        public bool IsFireInput
        {
            get
            {
                return _isFireInput;
            }

            private set
            {
                if (_isFireInput == value)
                {
                    return;
                }

                _isFireInput = value;
                
                OnFireInputChanged?.Invoke();
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
            _actions.Move.started += OnMoveInput;
            _actions.Move.performed += OnMoveInput;
            _actions.Move.canceled += OnMoveInput;
            
            _actions.Fire.started += OnFireInput;
            _actions.Fire.canceled += OnFireInput;
        }

        private void Unsubscribe()
        {
            _actions.Move.started -= OnMoveInput;
            _actions.Move.performed -= OnMoveInput;
            _actions.Move.canceled -= OnMoveInput;
            
            _actions.Fire.started -= OnFireInput;
            _actions.Fire.canceled -= OnFireInput;
        }

        private void OnMoveInput(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnFireInput(InputAction.CallbackContext context)
        {
            IsFireInput = context.ReadValueAsButton();
        }
    }
}
