using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TrailingBraceModule : ToolModuleBase
{
    private List<TrailingIssue> issues = new List<TrailingIssue>();

    public TrailingBraceModule()
    {
        Name = "Фигурные скобки";
    }

    public override void Draw()
    {
        if (GUILayout.Button("Сканировать скрипты на пустые строки после последней }"))
        {
            ScanAllScripts();
        }

        GUILayout.Label($"Найдено: {issues.Count}", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var i in issues)
        {
            if (i.ignored) continue;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("⚠", GUILayout.Width(20));
            GUILayout.Label(i.path, GUILayout.MinWidth(300));
            if (GUILayout.Button("Открыть", GUILayout.Width(60)))
            {
                OpenFile(i.path, 1);
            }
            if (GUILayout.Button("✔", GUILayout.Width(25)))
            {
                i.ignored = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void ScanAllScripts()
    {
        issues.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".cs")) continue;
            CheckFile(path);
        }
        Debug.Log($"Сканирование завершено. Найдено: {issues.Count}");
    }

    private void CheckFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;
        string content = File.ReadAllText(fullPath);
        int lastBrace = content.LastIndexOf('}');
        if (lastBrace < 0) return;
        string after = content.Substring(lastBrace + 1);
        if (string.IsNullOrEmpty(after)) return;
        string[] lines = after.Split('\n');
        foreach (var line in lines)
            if (!string.IsNullOrWhiteSpace(line))
            {
                issues.Add(new TrailingIssue { path = assetPath });
                return;
            }
        int emptyLines = lines.Count(l => string.IsNullOrWhiteSpace(l));
        if (emptyLines >= 2)
            issues.Add(new TrailingIssue { path = assetPath });
    }

    private class TrailingIssue
    {
        public string path;
        public bool ignored;
    }
}