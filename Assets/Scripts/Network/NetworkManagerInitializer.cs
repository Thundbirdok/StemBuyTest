namespace Network
{
    using Relay;
    using UnityEngine;
    using Unity.Netcode;
    using Unity.Netcode.Transports.UTP;
    using Unity.Services.Core;
    using Unity.Services.Lobbies;
    using Unity.Services.Relay;

    public class NetworkManagerInitializer : MonoBehaviour
    {
        [SerializeField] 
        private NetworkManager networkManager;

        [SerializeField]
        private UnityTransport transport;

        [SerializeField]
        private ConnectionController connectionController;
        
        public void Start()
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                return;   
            }
            
            connectionController.Enable();
            
            if (RelayHandler.IsHost)
            {
                StartGame();
                
                return;
            }
            
            JoinGame();
        }

        public void OnDestroy()
        {
            connectionController.Disable();
        }

        private void StartGame()
        {
            try
            {
                var allocation = RelayHandler.Allocation;
                
                transport
                    .SetHostRelayData
                    (
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData
                    );
                
                networkManager.StartHost();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }
        
        private async void JoinGame()
        {
            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync
                (
                    RelayHandler.JoinCode
                );
                
                transport
                    .SetClientRelayData
                    (
                        allocation.RelayServer.IpV4,
                        (ushort)allocation.RelayServer.Port,
                        allocation.AllocationIdBytes,
                        allocation.Key,
                        allocation.ConnectionData,
                        allocation.HostConnectionData
                    );
                
                networkManager.StartClient();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }
    }
}
