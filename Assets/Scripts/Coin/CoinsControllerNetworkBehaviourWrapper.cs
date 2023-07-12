using Unity.Netcode;

namespace Coin
{
    public class CoinsControllerNetworkBehaviourWrapper : NetworkBehaviour
    {
        public NetworkVariable<ushort> Coins { get; } = new NetworkVariable<ushort>();
    }
}
