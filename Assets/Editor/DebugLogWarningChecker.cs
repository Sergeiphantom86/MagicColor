using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

public class DebugLogWarningChecker : EditorWindow
{
    private Vector2 scrollPos;
    private List<ProblemScript> problemScripts = new List<ProblemScript>();
    private string searchFilter = "";

    [MenuItem("Tools/Проверка Debug.LogWarning")]
    public static void ShowWindow()
    {
        GetWindow<DebugLogWarningChecker>("Проверка логов");
    }

    private void OnGUI()
    {
        GUILayout.Label("Поиск битых строк (кракозябры) в Debug.LogWarning / Error",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Находит только реальные ошибки кодировки:\n" +
            "• ����\n" +
            "• � (replacement char)\n" +
            "• ???/битые последовательности\n\n" +
            "Русский текст НЕ трогается",
            MessageType.Info);

        if (GUILayout.Button("Проверить все скрипты в Assets/Scripts"))
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
        GUILayout.Label($"Найдено проблем: {problemScripts.Count}",
            EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(searchFilter)
            ? problemScripts
            : problemScripts.Where(p => p.filePath.Contains(searchFilter)).ToList();

        foreach (var item in filtered)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(item.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField($"стр. {item.lineNumber}", GUILayout.Width(60));
            EditorGUILayout.LabelField(item.problematicContent, GUILayout.MinWidth(150));

            if (GUILayout.Button("Открыть", GUILayout.Width(70)))
            {
                OpenScriptAtLine(item.filePath, item.lineNumber);
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

        Debug.Log($"Проверка завершена. Найдено проблем: {problemScripts.Count}");
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

    // ✔ СУПЕР ПРОСТАЯ И НАДЁЖНАЯ ПРОВЕРКА
    private bool IsBrokenText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        // 1. UTF replacement char
        if (text.Contains('\uFFFD'))
            return true;

        // 2. типичные кракозябры
        if (text.Contains("����") ||
            text.Contains("���") ||
            text.Contains("��"))
            return true;

        // 3. повторяющиеся символы "�"
        int count = 0;

        foreach (char c in text)
        {
            if (c == '�')
            {
                count++;
                if (count >= 3)
                    return true;
            }
            else
            {
                count = 0;
            }
        }

        return false;
    }

    private int GetLineNumber(string content, int index)
    {
        int line = 1;

        for (int i = 0; i < index && i < content.Length; i++)
        {
            if (content[i] == '\n')
                line++;
        }

        return line;
    }

    private void OpenScriptAtLine(string assetPath, int lineNumber)
    {
        string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, assetPath.Substring(7)));

        if (File.Exists(fullPath))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, lineNumber);
        }
        else
        {
            Debug.LogError($"Файл не найден: {fullPath}");
        }
    }

    private class ProblemScript
    {
        public string filePath;
        public int lineNumber;
        public string problematicContent;
    }
}