namespace Lobby
{
    using System;
    using TMPro;
    using UnityEngine;
    using Unity.Services.Lobbies.Models;
    using UnityEngine.UI;

    public class LobbyPanel : MonoBehaviour
    {
        public event Action<Lobby> OnJoinLobby;
        
        [SerializeField]
        private TextMeshProUGUI lobbyName;
        
        [SerializeField]
        private TextMeshProUGUI amount;
        
        [SerializeField]
        private TextMeshProUGUI maxAmount;

        [SerializeField]
        private Button button;

        private Lobby _lobby;
        
        private void OnEnable()
        {
            button.onClick.AddListener(Join);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Join);
        }

        public void Initialize(Lobby lobby)
        {
            _lobby = lobby;
            
            lobbyName.text = lobby.Name;
            amount.text = lobby.Players.Count.ToString();
            maxAmount.text = lobby.MaxPlayers.ToString();
        }

        private void Join() => OnJoinLobby?.Invoke(_lobby);
    }
}
