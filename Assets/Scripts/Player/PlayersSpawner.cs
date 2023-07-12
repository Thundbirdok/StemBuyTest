namespace Player
{
    using Network;
    using UnityEngine;
    using Unity.Netcode;
    using System.Collections.Generic;

    public class PlayersSpawner : NetworkBehaviour
    {
        [SerializeField]
        private PlayerCharacterController playerPrefab;

        [SerializeField]
        private Transform[] spawnPoints;

        private int _playersAmount;
        
        private readonly Dictionary<ulong, PlayerCharacterController> _players 
            = new Dictionary<ulong, PlayerCharacterController>();
        
        public override void OnNetworkSpawn()
        {
            if (IsServer == false)
            {
                return;
            }
            
            if (NetworkObjectPool.IsPrefabsRegistered == false)
            {
                NetworkObjectPool.OnPrefabsRegistered += Enable;
                
                return;
            }
            
            Enable();            
        }
        
        public override void OnNetworkDespawn()
        {
            if (IsServer == false)
            {
                return;
            }
            
            NetworkObjectPool.OnPrefabsRegistered -= Enable;
            
            if (NetworkManager.Singleton == null)
            {
                return;
            }
            
            NetworkManager.Singleton.OnClientConnectedCallback -= OnPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnPlayerDisconnected;
        }

        private void Enable()
        {
            NetworkObjectPool.OnPrefabsRegistered -= Enable;
            
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnected;
            
            if (IsHost && _playersAmount == 0)
            {
                OnPlayerConnected(NetworkManager.Singleton.LocalClientId);
            }
        }
        
        private void OnPlayerConnected(ulong id)
        {
            _playersAmount++;

            SpawnPlayer(id, _playersAmount - 1);
        }
        
        private void OnPlayerDisconnected(ulong id)
        {
            _playersAmount--;
            
            DespawnPlayer(id);
        }

        private void SpawnPlayer(ulong id, int spawnPointIndex)
        {
            var player = NetworkObjectPool.Singleton.GetNetworkObject
            (
                playerPrefab.gameObject,
                spawnPoints[spawnPointIndex % spawnPoints.Length].position,
                Quaternion.identity
            );
            
            player.name = $"Player {id}";
            
            player.Spawn(true);
            
            _players.Add(id, player.GetComponent<PlayerCharacterController>());
        }

        private void DespawnPlayer(ulong id)
        {
            if (_players.TryGetValue(id, out var player) == false)
            {
                return;
            }

            player.NetworkObject.Despawn();
            _players.Remove(id);
        }
    }
}
