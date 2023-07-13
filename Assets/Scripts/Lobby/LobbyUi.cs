namespace Lobby
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using Unity.Services.Lobbies.Models;

    public class LobbyUi : MonoBehaviour
    {
        [SerializeField]
        private LobbiesController controller;
        
        [SerializeField]
        private LobbiesList list;

        [SerializeField]
        private TMP_InputField searchField;

        [SerializeField]
        private Button searchButton;

        [SerializeField]
        private Button createButton;

        [SerializeField]
        private TMP_InputField createNameField;
        
        [SerializeField]
        private GameObject hostLobbyContainer;
        
        [SerializeField]
        private LobbyPanel hostLobbyPanel;
        
        private void OnEnable()
        {
            searchField.text = "";

            searchButton.onClick.AddListener(Populate);
            
            controller.OnHostLobbyStatusChange += ShowHostLobby;
            
            createButton.onClick.AddListener(CreateLobby);

            list.OnJoinLobby += JoinLobby;

            ShowHostLobby();
            Populate();
        }

        private void OnDisable()
        {
            searchButton.onClick.RemoveListener(Populate);
            
            controller.OnHostLobbyStatusChange -= ShowHostLobby;
            
            createButton.onClick.RemoveListener(CreateLobby);

            list.OnJoinLobby -= JoinLobby;
            
            list.Clear();
        }

        private void JoinLobby(Lobby lobby)
        {
            controller.JoinLobby(lobby);
        }

        private void Populate()
        {
            list.Populate(searchField.text);
        }

        private void CreateLobby()
        {
            if (string.IsNullOrEmpty(createNameField.text))
            {
                return;
            }
            
            controller.CreateLobby(createNameField.text);
        }
        
        private void ShowHostLobby()
        {
            if (controller.HostLobby == null)
            {
                hostLobbyContainer.SetActive(false);
                
                return;
            }
            
            hostLobbyPanel.Initialize(controller.HostLobby);
            hostLobbyContainer.SetActive(true);
        }
    }
}
