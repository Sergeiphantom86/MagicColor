using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

public class MenuLoader : MonoBehaviour
{
    private const string Menu = nameof(Menu);

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
            YG2.saves.SetIndexExit();
        }
    }
}