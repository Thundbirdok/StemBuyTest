namespace Network
{
    using Unity.Netcode;
    using UnityEngine;
    
    public class ConnectionController : MonoBehaviour
    {
        [SerializeField]
        private int maxPlayers = 2;

        [SerializeField]
        private Transform[] spawnPoints;

        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        }
        
        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null)
            {
                return;   
            }
            
            NetworkManager.Singleton.ConnectionApprovalCallback = null;
        }

        private void ApprovalCheck
        (
            NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response
        )
        {
            if (NetworkManager.Singleton.IsServer == false)
            {
                return;
            }
            
            var clientsCount = NetworkManager.Singleton.ConnectedClients.Count;

            if (clientsCount >= maxPlayers)
            {
                response.Approved = false;
                response.Pending = false;
                
                return;
            }
            
            response.Approved = true;

            var spawnPoint = spawnPoints[clientsCount % spawnPoints.Length];
            
            response.Position = spawnPoint.position;
            response.Rotation = spawnPoint.rotation;
            
            response.CreatePlayerObject = true;
            response.PlayerPrefabHash = null;
            
            response.Pending = false;
        }
    }
}
