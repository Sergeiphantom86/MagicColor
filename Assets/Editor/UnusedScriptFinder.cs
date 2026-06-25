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
        // 1. Получаем все MonoBehaviour-типы из скриптов в RootFolder
        var scriptsInFolder = AssetDatabase.FindAssets("t:MonoScript", new[] { RootFolder })
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Where(path => path.EndsWith(".cs"))
            .Select(path => AssetDatabase.LoadAssetAtPath<MonoScript>(path))
            .Where(script => script != null)
            .ToList();

        var scriptTypes = scriptsInFolder
            .Select(script => script.GetClass())
            .Where(type => type != null && type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract)
            .ToList();

        if (scriptTypes.Count == 0)
        {
            Debug.LogWarning($"В папке '{RootFolder}' не найдено классов MonoBehaviour.");
            return;
        }

        Debug.Log($"Найдено {scriptTypes.Count} классов MonoBehaviour в '{RootFolder}'.");

        // 2. Собираем типы, которые реально используются в префабах и сценах
        var usedTypes = new HashSet<Type>();

        // --- Сканируем префабы (они не вызывают проблем) ---
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int processed = 0;
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            var components = prefab.GetComponentsInChildren<Component>(true);
            foreach (var comp in components)
                if (comp != null) usedTypes.Add(comp.GetType());

            processed++;
            EditorUtility.DisplayProgressBar("Сканирование префабов", path, (float)processed / prefabGuids.Length);
        }

        // --- Сканируем сцены, но только те, что находятся в папке Assets (исключаем Packages) ---
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            // Пропускаем сцены из папок Packages, чтобы избежать ошибок
            if (path.StartsWith("Packages/"))
                continue;

            // Пропускаем сцены с некорректными расширениями (например, .unity)
            if (!path.EndsWith(".unity"))
                continue;

            Scene scene = default;
            bool sceneLoaded = false;
            try
            {
                // Открываем сцену в режиме Additive, чтобы не выгружать текущую
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                sceneLoaded = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Не удалось открыть сцену '{path}': {e.Message}. Пропускаем.");
                continue;
            }

            if (!scene.IsValid())
                continue;

            try
            {
                var rootObjects = scene.GetRootGameObjects();
                foreach (var go in rootObjects)
                {
                    var components = go.GetComponentsInChildren<Component>(true);
                    foreach (var comp in components)
                        if (comp != null) usedTypes.Add(comp.GetType());
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Ошибка при обработке сцены '{path}': {e.Message}. Пропускаем.");
            }
            finally
            {
                // Закрываем сцену, если она была открыта
                if (sceneLoaded && scene.IsValid())
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

            processed++;
            EditorUtility.DisplayProgressBar("Сканирование сцен", path, (float)processed / sceneGuids.Length);
        }

        EditorUtility.ClearProgressBar();

        // 3. Находим типы из нашей папки, которые не используются
        var unusedTypes = scriptTypes
            .Where(t => !usedTypes.Contains(t))
            .ToList();

        if (unusedTypes.Count == 0)
        {
            Debug.Log($"✅ Все скрипты в '{RootFolder}' используются.");
        }
        else
        {
            Debug.LogWarning($"⚠️ Найдено {unusedTypes.Count} неиспользуемых скриптов в '{RootFolder}':");
            foreach (var type in unusedTypes)
            {
                string scriptName = scriptsInFolder.FirstOrDefault(s => s.GetClass() == type)?.name ?? type.Name;
                Debug.Log($"   - {type.FullName} (файл: {scriptName}.cs)");
            }
        }
    }
}