namespace ScenesManagement
{
    using UnityEngine;
    using System.Threading.Tasks;
    using UnityEngine.SceneManagement;

    public class Loading : MonoBehaviour
    {
        [SerializeField]
        private float minimumLoadingTime;

        private const string LOADING_SCENE_NAME = "Loading";
        
        private static string _targetScene;
        
        private bool _isSceneActivationAllowed;
        
        private void Start()
        {
            LoadTargetScene();
        }

        private void OnDestroy() => Bootstrap.OnDone -= ActivateScene;

        public static void LoadScene(string targetScene)
        {
            _targetScene = targetScene;

            var loading = SceneManager.LoadSceneAsync(LOADING_SCENE_NAME);
            loading.allowSceneActivation = true;
        }

        private void LoadTargetScene()
        {
            LoadScene();
            
            if (Bootstrap.IsDone)
            {
                ActivateScene();
                
                return;
            }

            Bootstrap.OnDone += ActivateScene;
        }
        
        private void ActivateScene()
        {
            Bootstrap.OnDone -= ActivateScene;
            
            _isSceneActivationAllowed = true;
        }

        private async void LoadScene()
        {
            if (string.IsNullOrEmpty(_targetScene))
            {
                Debug.Log("Target scene is not set");
                
                return;
            }
            
            _isSceneActivationAllowed = false;
            
            var lobbyScene = SceneManager.LoadSceneAsync(_targetScene);

            lobbyScene.allowSceneActivation = false;
            
            var time = 0f;
            
            while (IsReadyToActivateScene(time, lobbyScene.progress))
            {
                time += Time.unscaledDeltaTime;
                
                await Task.Yield();
            }
            
            lobbyScene.allowSceneActivation = true;
        }

        private bool IsReadyToActivateScene(float time, float progress)
        {
            return _isSceneActivationAllowed == false 
                   || time < minimumLoadingTime 
                   || progress < 0.9f;
        }
    }
}
