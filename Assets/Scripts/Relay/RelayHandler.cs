namespace Relay
{
    using Unity.Services.Relay.Models;

    public static class RelayHandler
    {
        public static bool IsHost;
        public static string JoinCode;
        public static Allocation Allocation;
        
        public static void Set(bool isHost, string joinCode, Allocation allocation)
        {
            IsHost = isHost;
            JoinCode = joinCode;
            Allocation = allocation;
        }
    }
}
