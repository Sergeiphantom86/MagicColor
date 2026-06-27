using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;

public class UnusedScriptModule : ToolModuleBase
{
    private string resultMessage = "";

    public UnusedScriptModule()
    {
        Name = "Неиспользуемые скрипты";
    }

    public override void Draw()
    {
        if (GUILayout.Button("Найти неиспользуемые скрипты в Assets/Scripts"))
        {
            resultMessage = FindUnusedScripts();
        }

        if (!string.IsNullOrEmpty(resultMessage))
        {
            EditorGUILayout.HelpBox(resultMessage, MessageType.Info);
        }
    }

    private string FindUnusedScripts()
    {
        var scriptTypes = GetMonoBehaviourTypes();
        if (scriptTypes.Count == 0)
            return "В папке Assets/Scripts не найдено классов MonoBehaviour.";

        var usedTypes = CollectUsedTypes();
        var unusedTypes = scriptTypes.Where(t => !usedTypes.Contains(t)).ToList();

        if (unusedTypes.Count == 0)
            return "✅ Все скрипты в Assets/Scripts используются.";

        string result = $"⚠️ Найдено {unusedTypes.Count} неиспользуемых скриптов:\n";
        foreach (var type in unusedTypes)
            result += $"   - {type.FullName}\n";
        return result;
    }

    private List<Type> GetMonoBehaviourTypes()
    {
        return AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets/Scripts" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".cs"))
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .Where(s => s != null)
            .Select(s => s.GetClass())
            .Where(t => t != null && t.IsSubclassOf(typeof(MonoBehaviour)) && !t.IsAbstract)
            .ToList();
    }

    private HashSet<Type> CollectUsedTypes()
    {
        var used = new HashSet<Type>();
        // Prefabs
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;
            foreach (var comp in prefab.GetComponentsInChildren<Component>(true))
                if (comp != null) used.Add(comp.GetType());
            EditorUtility.DisplayProgressBar("Сканирование префабов", path, (float)i / prefabGuids.Length);
        }
        // Scenes
        var sceneGuids = AssetDatabase.FindAssets("t:Scene");
        for (int i = 0; i < sceneGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
            if (path.StartsWith("Packages/") || !path.EndsWith(".unity")) continue;
            Scene scene = default;
            bool loaded = false;
            try
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                loaded = true;
                if (!scene.IsValid()) continue;
                foreach (var go in scene.GetRootGameObjects())
                    foreach (var comp in go.GetComponentsInChildren<Component>(true))
                        if (comp != null) used.Add(comp.GetType());
            }
            catch (Exception e) { Debug.LogWarning($"Ошибка сцены '{path}': {e.Message}"); }
            finally
            {
                if (loaded && scene.IsValid())
                    try { EditorSceneManager.CloseScene(scene, true); }
                    catch (Exception e) { Debug.LogWarning($"Не удалось закрыть сцену '{path}': {e.Message}"); }
            }
            EditorUtility.DisplayProgressBar("Сканирование сцен", path, (float)i / sceneGuids.Length);
        }
        EditorUtility.ClearProgressBar();
        return used;
    }
}