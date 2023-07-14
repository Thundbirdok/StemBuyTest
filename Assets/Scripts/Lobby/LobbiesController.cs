using UnityEngine;

namespace Lobby
{
    using System;
    using System.Collections.Generic;
    using Network;
    using Relay;
    using ScenesManagement;
    using Unity.Services.Authentication;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using Unity.Services.Relay;

    public class LobbiesController : MonoBehaviour
    {
        public event Action OnHostLobbyStatusChange;

        private Lobby _hostLobby;
        public Lobby HostLobby
        {
            get
            {
                return _hostLobby;
            }

            private set
            {
                if (_hostLobby == value)
                {
                    return;
                }

                _hostLobby = value;
                OnHostLobbyStatusChange?.Invoke();
            }
        }

        public Lobby JoinedLobby { get; private set; }

        [SerializeField]
        private float lobbyHeartbeatDelay = 15f;

        [SerializeField]
        private float updateLobbyDelay = 5;
        
        [SerializeField]
        private int maxPlayers = 2;

        private const string KEY_START_GAME = "KeyStartGame";
        private const string GAME_SCENE_NAME = "Game";
        
        private float _heartbeatTime;

        private float _updateLobbyTime;

        private bool _isStartingGame;
        
        private void Update()
        {
            KeepLobbyHeartbeat();
            HandleLobbyUpdate();
        }

        public async void CreateLobby(string lobbyName)
        {
            try
            {
                var options = new CreateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            KEY_START_GAME, new DataObject
                            (
                                DataObject.VisibilityOptions.Member,
                                "0"
                            )
                        }
                    }
                };
                
                var lobby = await LobbyService.Instance.CreateLobbyAsync
                (
                    lobbyName,
                    maxPlayers,
                    options
                );

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

            JoinedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
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

            JoinedLobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);

            if (HostLobby != null)
            {
                HostLobby = JoinedLobby;
            
                TryStartGame();
            
                return;
            }

            var joinCode = JoinedLobby.Data[KEY_START_GAME].Value;

            if (joinCode == "0")
            {
                return;
            }

            JoinGame(joinCode);
        }

        private void JoinGame(string joinCode)
        {
            if (_isStartingGame)
            {
                return;
            }

            _isStartingGame = true;
            
            RelayHandler.Set(false, joinCode, null);

            LoadGame();
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

        private async void TryStartGame()
        {
            try
            {
                if (HostLobby.Players.Count < maxPlayers)
                {
                    return;
                }

                if (_isStartingGame)
                {
                    return;
                }

                _isStartingGame = true;
                
                var allocation = await RelayService.Instance
                    .CreateAllocationAsync(maxPlayers - 1);

                var joinCode = await RelayService.Instance
                    .GetJoinCodeAsync(allocation.AllocationId);

                HostLobby = await Lobbies.Instance.UpdateLobbyAsync
                (
                    HostLobby.Id,
                    new UpdateLobbyOptions
                    {
                        Data = new Dictionary<string, DataObject>
                        {
                            {
                                KEY_START_GAME,
                                new DataObject(DataObject.VisibilityOptions.Member, joinCode)
                            }
                        }
                    }
                );
                
                RelayHandler.Set(true, joinCode, allocation);
                
                LoadGame();
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        private static void LoadGame()
        {
            Loading.LoadScene(GAME_SCENE_NAME);
        }
    }
}
