namespace Lobby
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Unity.Services.Core;
    using Unity.Services.Lobbies;
    using Unity.Services.Lobbies.Models;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [Serializable]
    public class LobbiesList
    {
        public event Action<Lobby> OnJoinLobby;
        
        [SerializeField]
        private LobbyPanel lobbyPanelPrefab;
        
        [SerializeField]
        private Transform container;
        
        private List<Lobby> _foundedLobbies = new List<Lobby>();

        private List<LobbyPanel> _lobbyPanels = new List<LobbyPanel>();

        public async void Populate(string filter)
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                return;
            }
            
            await GetLobbiesWithFilter(filter);

            ClearPanels();
            PopulatePanels();
        }

        public void Clear()
        {
            ClearPanels();
        }
        
        private void PopulatePanels()
        {
            foreach (var lobby in _foundedLobbies)
            {
                var lobbyPanel = Object.Instantiate(lobbyPanelPrefab, container);
                lobbyPanel.transform.SetParent(container);
                lobbyPanel.Initialize(lobby);

                lobbyPanel.OnJoinLobby += Join;
                
                _lobbyPanels.Add(lobbyPanel);
            }
        }

        private void Join(Lobby lobby) => OnJoinLobby?.Invoke(lobby);

        private void ClearPanels()
        {
            for (var i = 0; i < _lobbyPanels.Count; )
            {
                _lobbyPanels[0].OnJoinLobby -= Join;
                
                Object.Destroy(_lobbyPanels[0].gameObject);
                _lobbyPanels.RemoveAt(i);
            }
        }

        private async Task GetLobbiesWithFilter(string filter)
        {
            try
            {
                var queryResponse = await GetLobbies(filter);

                _foundedLobbies = queryResponse.Results;
                
                foreach (var lobby in queryResponse.Results)
                {
                    Debug.Log(lobby);
                }
            }
            catch (LobbyServiceException exception)
            {
                Debug.Log(exception.Message);
                Debug.Log(exception.StackTrace);
            }
        }

        private async static Task<QueryResponse> GetLobbies(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return await GetAllLobbies();
            }
            
            return await GetLobbiesWithKeyInName(filter);
        }

        private async static Task<QueryResponse> GetLobbiesWithKeyInName(string key)
        {
            var options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.Name, key, QueryFilter.OpOptions.CONTAINS)
                }
            };

            return await Lobbies.Instance.QueryLobbiesAsync(options);
        }
        
        private async static Task<QueryResponse> GetAllLobbies()
        {
            return await Lobbies.Instance.QueryLobbiesAsync();
        }
    }
}
