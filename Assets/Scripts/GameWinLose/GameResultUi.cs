namespace GameWinLose
{
    using Network;
    using ScenesManagement;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GameResultUi : MonoBehaviour
    {
        [SerializeField]
        private GameResultController controller;
        
        [SerializeField]
        private NetworkManagerInitializer networkManagerInitializer;
        
        [SerializeField]
        private GameObject screen;

        [SerializeField]
        private TextMeshProUGUI winText;
        
        [SerializeField]
        private TextMeshProUGUI loseText;

        [SerializeField]
        private Button loadLobby;

        private const string LOBBY_SCENE_NAME = "Lobby";
        
        private void OnEnable()
        {
            screen.SetActive(false);

            controller.OnGameResult += ShowResult;
            loadLobby.onClick.AddListener(LoadLobby);
        }

        private void OnDisable()
        {
            controller.OnGameResult -= ShowResult;
            loadLobby.onClick.RemoveListener(LoadLobby);
        }

        private void ShowResult()
        {
            screen.SetActive(true);
            
            winText.gameObject.SetActive(controller.IsWin);
            loseText.gameObject.SetActive(controller.IsWin == false);
        }

        private void LoadLobby()
        {
            networkManagerInitializer.Shutdown();
            
            Loading.LoadScene(LOBBY_SCENE_NAME);
        }
    }
}
