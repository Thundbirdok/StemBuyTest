namespace Health
{
    using Unity.Netcode;

    public class HealthNetworkVariableWrapper : NetworkBehaviour
    {
        public NetworkVariable<ushort> Health { get; } = new NetworkVariable<ushort>();
    }
}
