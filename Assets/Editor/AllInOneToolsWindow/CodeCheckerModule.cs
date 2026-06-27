using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class CodeCheckerModule : ToolModuleBase
{
    private List<CodeIssue> issues = new List<CodeIssue>();

    public CodeCheckerModule()
    {
        Name = "Проверка кода";
    }

    public override void Draw()
    {
        if (GUILayout.Button("Проверить все скрипты на цепочки и длину строк"))
        {
            CheckAllScripts();
        }

        GUILayout.Space(10);
        DrawFilter();

        GUILayout.Label($"Найдено проблем: {issues.Count}", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(filter)
            ? issues
            : issues.Where(i => MatchesFilter(i.filePath, filter)).ToList();

        foreach (var item in filtered)
        {
            if (item.ignored) continue;
            EditorGUILayout.BeginHorizontal();
            string icon = item.isLongLine ? "⚠️" : "🔴";
            EditorGUILayout.LabelField(icon, GUILayout.Width(25));
            string label = $"стр. {item.line}";
            if (item.isLongLine) label += $"  >120 ({item.charCount})";
            EditorGUILayout.LabelField(item.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField(label, GUILayout.Width(180));
            EditorGUILayout.LabelField(item.snippet, GUILayout.MinWidth(200));
            if (GUILayout.Button("Открыть", GUILayout.Width(60)))
            {
                OpenFile(item.filePath, item.line);
            }
            if (GUILayout.Button("✔", GUILayout.Width(25)))
            {
                item.ignored = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void CheckAllScripts()
    {
        issues.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".cs"))
                CheckFile(path);
        }
        Debug.Log($"Проверка кода завершена. Найдено проблем: {issues.Count}");
    }

    private void CheckFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;
        string content = File.ReadAllText(fullPath);

        string pattern1 = @"\.\s*\n\s*\w+\s*\(";
        var matches1 = Regex.Matches(content, pattern1, RegexOptions.Multiline);
        foreach (Match m in matches1)
            AddIssue(content, assetPath, m.Index);

        string pattern2 = @"\w+\s*\(\s*\n\s*";
        var matches2 = Regex.Matches(content, pattern2, RegexOptions.Multiline);
        foreach (Match m in matches2)
            AddIssue(content, assetPath, m.Index);
    }

    private void AddIssue(string content, string path, int index)
    {
        var issue = new CodeIssue
        {
            filePath = path,
            line = GetLineNumber(content, index),
            snippet = ExtractSnippet(content, index),
        };
        int start = content.LastIndexOf('\n', Mathf.Max(0, index));
        if (start < 0) start = 0;
        int end = content.IndexOf('\n', index);
        if (end < 0) end = content.Length;
        string line = content.Substring(start, end - start).Replace("\r", "").Replace("\n", "");
        issue.charCount = line.Length;
        issue.isLongLine = line.Length > 120;
        issues.Add(issue);
    }

    private string ExtractSnippet(string text, int index)
    {
        int start = Mathf.Max(0, index - 40);
        int end = Mathf.Min(text.Length, index + 80);
        return text.Substring(start, end - start).Replace("\n", " ↵ ");
    }

    private class CodeIssue
    {
        public string filePath;
        public int line;
        public string snippet;
        public bool isLongLine;
        public int charCount;
        public bool ignored;
    }
}