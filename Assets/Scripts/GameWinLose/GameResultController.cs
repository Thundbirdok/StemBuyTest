using UnityEngine;

namespace GameWinLose
{
    using System;
    using System.Collections.Generic;
    using Network;
    using Player;
    using Unity.Netcode;

    public class GameResultController : MonoBehaviour
    {
        public event Action OnGameResult;

        private bool _isWin;

        public bool IsWin
        {
            get
            {
                return _isWin;
            }

            private set
            {
                _isWin = value;
                OnGameResult?.Invoke();
            }
        }

        private readonly List<PlayerCharacterController> _players =
            new List<PlayerCharacterController>();
        
        private void OnEnable()
        {
            PlayerCharacterController.OnPlayerSpawned += OnPlayerSpawned;
        }

        private void OnDisable()
        {
            PlayerCharacterController.OnPlayerSpawned -= OnPlayerSpawned;

            for (var i = 0; i < _players.Count; )
            {
                _players[0].HealthController.OnDeath -= OnPlayerDeath;
                _players.RemoveAt(0);
            }
        }

        private void OnPlayerSpawned(PlayerCharacterController player)
        {
            Debug.Log("Player spawned: " + player.OwnerClientId);
            
            player.HealthController.OnDeath += OnPlayerDeath;
            
            _players.Add(player);
        }

        private void OnPlayerDeath(PlayerCharacterController player)
        {
            player.HealthController.OnDeath -= OnPlayerDeath;
            
            if (player.IsOwner)
            {
                IsWin = false;
                
                _players.Remove(player);

                Debug.Log("Lose");
                
                return;
            }

            _players.Remove(player);
            
            var playersCount = _players.Count;
            Debug.Log("Player left: " + playersCount);
            
            if (playersCount == 1)
            {
                IsWin = true;

                Debug.Log("Win");
            }
        }
    }
}
