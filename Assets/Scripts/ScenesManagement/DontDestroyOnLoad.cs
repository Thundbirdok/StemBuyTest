using UnityEngine;

namespace ScenesManagement
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
        }
    }
}
