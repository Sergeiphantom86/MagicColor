using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class FluentChainBreakChecker : EditorWindow
{
    private Vector2 scrollPos;
    private List<Issue> issues = new List<Issue>();
    private string searchFilter = "";

    //[MenuItem("Tools/Средство проверки кода (цепочки + длина)")]
    public static void ShowWindow()
    {
        GetWindow<FluentChainBreakChecker>("Code Checker");
    }

    private void OnGUI()
    {
        GUILayout.Label("Проверка цепочек и длины строк", EditorStyles.boldLabel);

        if (GUILayout.Button("Проверить все скрипты"))
        {
            CheckAllScripts();
        }

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Фильтр:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter);

        if (GUILayout.Button("Очистить", GUILayout.Width(70)))
            searchFilter = "";

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.Label($"Найдено проблем: {issues.Count}", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(searchFilter)
            ? issues
            : issues.Where(i => i.filePath.Contains(searchFilter)).ToList();

        foreach (var item in filtered)
        {
            if (item.ignored)
                continue;

            EditorGUILayout.BeginHorizontal();

            // 🔥 Иконка статуса
            string icon = item.isLongLine ? "⚠️" : "🔴";

            EditorGUILayout.LabelField(icon, GUILayout.Width(25));

            string label = $"стр. {item.line}";

            if (item.isLongLine)
                label += $"  >120 ({item.charCount})";

            EditorGUILayout.LabelField(item.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField(label, GUILayout.Width(180));
            EditorGUILayout.LabelField(item.snippet, GUILayout.MinWidth(200));

            if (GUILayout.Button("Open", GUILayout.Width(60)))
            {
                OpenFile(item.filePath, item.line);
            }

            // ✔ "забыть"
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

        Debug.Log($"Готово. Найдено проблем: {issues.Count}");
    }

    private void CheckFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;

        string content = File.ReadAllText(fullPath);

        // 🔥 1. chain break
        string pattern1 = @"\.\s*\n\s*\w+\s*\(";

        var matches1 = Regex.Matches(content, pattern1, RegexOptions.Multiline);

        foreach (Match m in matches1)
        {
            var issue = CreateIssue(content, assetPath, m.Index, "CHAIN");
            issues.Add(issue);
        }

        // 🔥 2. multiline call
        string pattern2 = @"\w+\s*\(\s*\n\s*";

        var matches2 = Regex.Matches(content, pattern2, RegexOptions.Multiline);

        foreach (Match m in matches2)
        {
            var issue = CreateIssue(content, assetPath, m.Index, "MULTILINE");
            issues.Add(issue);
        }
    }

    private Issue CreateIssue(string content, string path, int index, string type)
    {
        var issue = new Issue
        {
            filePath = path,
            line = GetLine(content, index),
            snippet = Extract(content, index),
            issueType = type
        };

        CheckLength(content, index, issue);

        return issue;
    }

    private void CheckLength(string content, int index, Issue issue)
    {
        int start = content.LastIndexOf('\n', Mathf.Max(0, index));
        if (start < 0) start = 0;

        int end = content.IndexOf('\n', index);
        if (end < 0) end = content.Length;

        string line = content.Substring(start, end - start);

        string normalized = line.Replace("\r", "").Replace("\n", "");

        issue.charCount = normalized.Length;

        if (normalized.Length > 120)
            issue.isLongLine = true;
    }

    private int GetLine(string text, int index)
    {
        int line = 1;

        for (int i = 0; i < index && i < text.Length; i++)
            if (text[i] == '\n')
                line++;

        return line;
    }

    private string Extract(string text, int index)
    {
        int start = Mathf.Max(0, index - 40);
        int end = Mathf.Min(text.Length, index + 80);

        return text.Substring(start, end - start)
            .Replace("\n", " ↵ ");
    }

    private void OpenFile(string path, int line)
    {
        string full = Path.GetFullPath(
            Path.Combine(Application.dataPath, path.Substring(7)));

        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(full, line);
    }

    private class Issue
    {
        public string filePath;
        public int line;
        public string snippet;

        public string issueType;

        public bool isLongLine;
        public int charCount;

        public bool ignored;
    }
}