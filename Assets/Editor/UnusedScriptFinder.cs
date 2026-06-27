using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class UnusedScriptFinder
{
    private const string RootFolder = "Assets/Scripts";

    [MenuItem("Tools/Find Unused Scripts in " + RootFolder)]
    public static void FindUnusedScripts()
    {
        var scriptTypes = GetMonoBehaviourTypes();

        if (scriptTypes.Count == 0)
        {
            Debug.LogWarning($"В папке '{RootFolder}' не найдено классов MonoBehaviour.");
            return;
        }

        Debug.Log($"Найдено {scriptTypes.Count} классов MonoBehaviour в '{RootFolder}'.");

        var usedTypes = CollectUsedTypes();

        var unusedTypes = scriptTypes
            .Where(t => !usedTypes.Contains(t))
            .ToList();

        if (unusedTypes.Count == 0)
        {
            Debug.Log($"✅ Все скрипты в '{RootFolder}' используются.");
            return;
        }

        Debug.LogWarning($"⚠️ Найдено {unusedTypes.Count} неиспользуемых скриптов в '{RootFolder}':");

        foreach (var type in unusedTypes)
        {
            string scriptName = type.Name;
            Debug.Log($"   - {type.FullName} (файл: {scriptName}.cs)");
        }
    }

    private static List<Type> GetMonoBehaviourTypes()
    {
        return AssetDatabase.FindAssets("t:MonoScript", new[] { RootFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".cs"))
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .Where(s => s != null)
            .Select(s => s.GetClass())
            .Where(t => t != null && t.IsSubclassOf(typeof(MonoBehaviour)) && !t.IsAbstract)
            .ToList();
    }

    private static HashSet<Type> CollectUsedTypes()
    {
        var used = new HashSet<Type>();

        CollectFromPrefabs(used);
        CollectFromScenes(used);

        return used;
    }

    private static void CollectFromPrefabs(HashSet<Type> used)
    {
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        for (int i = 0; i < prefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                continue;

            foreach (var comp in prefab.GetComponentsInChildren<Component>(true))
            {
                if (comp != null)
                    used.Add(comp.GetType());
            }

            EditorUtility.DisplayProgressBar(
                "Сканирование префабов",
                path,
                (float)i / prefabGuids.Length);
        }
    }

    private static void CollectFromScenes(HashSet<Type> used)
    {
        var sceneGuids = AssetDatabase.FindAssets("t:Scene");

        for (int i = 0; i < sceneGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);

            if (path.StartsWith("Packages/") || !path.EndsWith(".unity"))
                continue;

            Scene scene = default;
            bool loaded = false;

            try
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                loaded = true;

                if (!scene.IsValid())
                    continue;

                foreach (var go in scene.GetRootGameObjects())
                {
                    foreach (var comp in go.GetComponentsInChildren<Component>(true))
                    {
                        if (comp != null)
                            used.Add(comp.GetType());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Ошибка сцены '{path}': {e.Message}");
            }
            finally
            {
                if (loaded && scene.IsValid())
                {
                    try
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Не удалось закрыть сцену '{path}': {e.Message}");
                    }
                }
            }

            EditorUtility.DisplayProgressBar(
                "Сканирование сцен",
                path,
                (float)i / sceneGuids.Length);
        }

        EditorUtility.ClearProgressBar();
    }
}