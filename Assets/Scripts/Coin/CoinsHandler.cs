namespace Coin
{
    using System;
    using Unity.Netcode;

    public class CoinsHandler : NetworkBehaviour
    {
        public event Action OnCoinsChanged;
        
        public ushort Coins => _coinsVariable.Value;

        private readonly NetworkVariable<ushort> _coinsVariable = new NetworkVariable<ushort>();

        public void Enable()
        {
            _coinsVariable.OnValueChanged += InvokeOnCoinsChanged;
        }

        public void Disable()
        {
            _coinsVariable.OnValueChanged -= InvokeOnCoinsChanged;
        }

        public void Add(ushort coins)
        {
            if (coins <= 0)
            {
                return;
            }

            _coinsVariable.Value += coins;
        }

        private void InvokeOnCoinsChanged(ushort _1, ushort _2)
        {
            OnCoinsChanged?.Invoke();
        }
    }
}
