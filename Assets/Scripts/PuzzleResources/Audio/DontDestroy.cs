using UnityEngine;

namespace PuzzleResources.Audio
{
    public class DontDestroy : MonoBehaviour
    {
        private static DontDestroy s_instance;

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                s_instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}