using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TrailingBraceCleaner : EditorWindow
{
    private Vector2 scroll;
    private List<Issue> issues = new List<Issue>();

    [MenuItem("Tools/Trailing } Checker")]
    public static void ShowWindow()
    {
        GetWindow<TrailingBraceCleaner>("Trailing Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Проверка пустых строк после последней }", EditorStyles.boldLabel);

        if (GUILayout.Button("Сканировать"))
        {
            Scan();
        }

        GUILayout.Space(5);
        GUILayout.Label($"Найдено: {issues.Count}", EditorStyles.boldLabel);

        scroll = GUILayout.BeginScrollView(scroll);

        foreach (var i in issues)
        {
            if (i.ignored)
                continue;

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("⚠", GUILayout.Width(20));
            GUILayout.Label(i.path, GUILayout.MinWidth(300));

            if (GUILayout.Button("Open", GUILayout.Width(60)))
            {
                Open(i.path);
            }

            // ✔ кнопка "забыть"
            if (GUILayout.Button("✔", GUILayout.Width(25)))
            {
                i.ignored = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void Scan()
    {
        issues.Clear();

        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".cs")) continue;

            CheckFile(path);
        }

        Debug.Log($"Done. Found: {issues.Count}");
    }

    private void CheckFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;

        string content = File.ReadAllText(fullPath);

        int lastBrace = content.LastIndexOf('}');
        if (lastBrace < 0) return;

        string after = content.Substring(lastBrace + 1);

        if (string.IsNullOrEmpty(after))
            return;

        string[] lines = after.Split('\n');

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                issues.Add(new Issue
                {
                    path = assetPath
                });
                return;
            }
        }

        int emptyLines = lines.Count(l => string.IsNullOrWhiteSpace(l));

        if (emptyLines >= 2)
        {
            issues.Add(new Issue
            {
                path = assetPath
            });
        }
    }

    private void Open(string assetPath)
    {
        string fullPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, assetPath.Substring(7)));

        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, 1);
    }

    private class Issue
    {
        public string path;
        public bool ignored;
    }
}