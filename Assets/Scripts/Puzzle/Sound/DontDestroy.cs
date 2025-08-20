using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy _instance;
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}