namespace ScenesManagement
{
    using System;
    using System.Threading.Tasks;
    using Lobby;
    using UnityEngine;

    public class Bootstrap : MonoBehaviour
    {
        public static event Action OnDone;

        public static bool IsDone { get; private set; }
        
        private const string LOBBY_SCENE_NAME = "Lobby";
        
        private async void Start()
        {
            IsDone = false;
            
            DontDestroyOnLoad(gameObject);
            
            var unityServices = UnityServicesInitializer.Initialize();
            
            Loading.LoadScene(LOBBY_SCENE_NAME);

            while (unityServices.IsCompleted == false)
            {
                await Task.Yield();
            }

            IsDone = true;
            OnDone?.Invoke();
            
            Destroy(gameObject);
        }
    }
}
