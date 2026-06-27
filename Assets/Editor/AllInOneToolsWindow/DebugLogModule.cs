using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class DebugLogModule : ToolModuleBase
{
    private List<ProblemScript> problemScripts = new List<ProblemScript>();

    public DebugLogModule()
    {
        Name = "Логи";
    }

    public override void Draw()
    {
        EditorGUILayout.HelpBox(
            "Находит битые строки (кракозябры) в Debug.LogWarning / Error",
            MessageType.Info);

        if (GUILayout.Button("Проверить все скрипты в Assets/Scripts"))
        {
            CheckAllScripts();
        }

        GUILayout.Space(10);
        DrawFilter();

        GUILayout.Label($"Найдено проблем: {problemScripts.Count}", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(filter)
            ? problemScripts
            : problemScripts.Where(p => MatchesFilter(p.filePath, filter)).ToList();

        foreach (var item in filtered)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField($"стр. {item.lineNumber}", GUILayout.Width(60));
            EditorGUILayout.LabelField(item.problematicContent, GUILayout.MinWidth(150));
            if (GUILayout.Button("Открыть", GUILayout.Width(70)))
            {
                OpenFile(item.filePath, item.lineNumber);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void CheckAllScripts()
    {
        problemScripts.Clear();
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { "Assets/Scripts" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith(".cs"))
                CheckScript(path);
        }
        Debug.Log($"Проверка Debug.Log завершена. Найдено проблем: {problemScripts.Count}");
    }

    private void CheckScript(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;

        string content = File.ReadAllText(fullPath);
        var patterns = new[]
        {
            @"Debug\.LogWarning\s*\(\s*""((?:[^""\\]|\\.)*)""",
            @"Debug\.LogWarning\s*\(\s*@""((?:[^""]|"")*)""",
            @"Debug\.LogWarning\s*\(\s*\$""((?:[^""\\]|\\.)*)""",
            @"Debug\.LogError\s*\(\s*""((?:[^""\\]|\\.)*)""",
            @"Debug\.LogError\s*\(\s*@""((?:[^""]|"")*)""",
            @"Debug\.LogError\s*\(\s*\$""((?:[^""\\]|\\.)*)"""
        };

        foreach (string pattern in patterns)
        {
            var matches = Regex.Matches(content, pattern);
            foreach (Match match in matches)
            {
                string text = match.Groups[1].Value;
                if (IsBrokenText(text))
                {
                    problemScripts.Add(new ProblemScript
                    {
                        filePath = assetPath,
                        lineNumber = GetLineNumber(content, match.Index),
                        problematicContent = text
                    });
                }
            }
        }
    }

    private bool IsBrokenText(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;
        if (text.Contains('\uFFFD')) return true;
        if (text.Contains("����") || text.Contains("���") || text.Contains("��")) return true;
        int count = 0;
        foreach (char c in text)
        {
            if (c == '�')
            {
                count++;
                if (count >= 3) return true;
            }
            else count = 0;
        }
        return false;
    }

    private class ProblemScript
    {
        public string filePath;
        public int lineNumber;
        public string problematicContent;
    }
}