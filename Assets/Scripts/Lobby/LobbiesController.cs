using UnityEngine;

namespace Lobby
{
    using System;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;

    public class LobbiesController : MonoBehaviour
    {
        public event Action OnHostLobbyStatusChange;

        public Lobby HostLobby { get; private set; }

        public Lobby JoinedLobby { get; private set; }
        
        [SerializeField]
        private float lobbyHeartbeatDelay = 15f;

        [SerializeField]
        private float updateLobbyDelay = 5;
        
        [SerializeField]
        private int maxPlayers = 2;

        private float _heartbeatTime;

        private float _updateLobbyTime;
        
        private void Update()
        {
            KeepLobbyHeartbeat();
            HandleLobbyUpdate();
        }

        public async void CreateLobby(string lobbyName)
        {
            try
            {
                var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

                HostLobby = lobby;
                JoinedLobby = lobby;
                
                OnHostLobbyStatusChange?.Invoke();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        public async void JoinLobby(Lobby lobby)
        {
            if (JoinedLobby != null || HostLobby != null)
            {
                return;
            }

            await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
        }

        public async void QuickJoin()
        {
            try
            {
                await Lobbies.Instance.QuickJoinLobbyAsync();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        public void LeaveLobby()
        {
            try
            {
                LobbyService.Instance.RemovePlayerAsync
                (
                    JoinedLobby.Id,
                    AuthenticationService.Instance.PlayerId
                );

                JoinedLobby = null;
                HostLobby = null;
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        private async void HandleLobbyUpdate()
        {
            if (JoinedLobby == null)
            {
                return;
            }

            _updateLobbyTime -= Time.unscaledDeltaTime;

            if (_updateLobbyTime > 0)
            {
                return;
            }

            _updateLobbyTime = updateLobbyDelay;

            await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
        }

        private async void KeepLobbyHeartbeat()
        {
            if (HostLobby == null)
            {
                return;
            }

            _heartbeatTime -= Time.unscaledDeltaTime;

            if (_heartbeatTime > 0)
            {
                return;
            }

            _heartbeatTime = lobbyHeartbeatDelay;

            await LobbyService.Instance.SendHeartbeatPingAsync(HostLobby.Id);
        }
    }
}
