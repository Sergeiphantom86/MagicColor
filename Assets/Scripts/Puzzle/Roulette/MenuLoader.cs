using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoader : MonoBehaviour
{
    private const string Menu = nameof(Menu);

    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
    }

    public void TargetScene(string scenName)
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found! Using default load.");
            SceneManager.LoadScene(Menu);
            return;
        }

        ReturnIndex();

        SceneLoader.Instance.LoadSceneWithSplash(scenName);
    }

    private void ReturnIndex()
    {
        if (gameObject.TryGetComponent(out Exit _))
        {
            _progressSaver.SetIndexExit();
        }
    }
}