using Unity.Netcode;

namespace Coin
{
    public class CoinsNetworkVariableWrapper : NetworkBehaviour
    {
        public NetworkVariable<ushort> Coins { get; } = new NetworkVariable<ushort>();
    }
}
