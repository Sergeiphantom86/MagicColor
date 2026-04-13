using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class EntryPointSceneAutoLoader
{
    private const string MenuPath = "Play/Enable";
    private const string PlayFromBootstrapKey = nameof(PlayFromBootstrapKey);
    private const int BootSceneIndex = 0;

    static EntryPointSceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChangeded;
    }

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool result = EditorPrefs.GetBool(PlayFromBootstrapKey);

        EditorPrefs.SetBool(PlayFromBootstrapKey, !result);
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PlayFromBootstrapKey));

        return true;
    }

    private static void OnPlayModeStateChangeded(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (EditorPrefs.GetBool(PlayFromBootstrapKey) == false)
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            if (EditorBuildSettings.scenes.Length == 0) return;

            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[BootSceneIndex].path);
        }
    }
}