namespace Coin
{
    using System;
    using UnityEngine;

    [Serializable]
    public class CoinsController
    {
        public event Action OnCoinsChanged;
        
        public ushort Coins => coinsWrapper.Coins.Value;

        [SerializeField]
        private CoinsControllerNetworkBehaviourWrapper coinsWrapper;

        public void Enable()
        {
            coinsWrapper.Coins.OnValueChanged += InvokeOnCoinsChanged;
        }

        public void Disable()
        {
            coinsWrapper.Coins.OnValueChanged -= InvokeOnCoinsChanged;
        }

        public void Add(ushort coins)
        {
            if (coins <= 0)
            {
                return;
            }

            coinsWrapper.Coins.Value += coins;
        }

        private void InvokeOnCoinsChanged(ushort _1, ushort _2)
        {
            OnCoinsChanged?.Invoke();
        }
    }
}
