using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Модуль для поиска всех Debug.Log/LogWarning/LogError/LogException в скриптах.
/// </summary>
public class FindAllDebugLogsModule : ToolModuleBase
{
    // Настройки поиска
    private bool includeLog = true;
    private bool includeWarning = true;
    private bool includeError = true;
    private bool includeException = true;
    private string searchFolder = "Assets/Scripts"; // можно изменить

    private List<DebugLogEntry> entries = new List<DebugLogEntry>();

    public FindAllDebugLogsModule()
    {
        Name = "Все Debug.Log";
    }

    public override void Draw()
    {
        EditorGUILayout.HelpBox(
            "Находит все вызовы Debug.Log, LogWarning, LogError и LogException в скриптах.",
            MessageType.Info);

        // Настройки
        EditorGUILayout.LabelField("Настройки поиска", EditorStyles.boldLabel);
        includeLog = EditorGUILayout.Toggle("Debug.Log", includeLog);
        includeWarning = EditorGUILayout.Toggle("Debug.LogWarning", includeWarning);
        includeError = EditorGUILayout.Toggle("Debug.LogError", includeError);
        includeException = EditorGUILayout.Toggle("Debug.LogException", includeException);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Папка поиска:", GUILayout.Width(100));
        searchFolder = EditorGUILayout.TextField(searchFolder);
        if (GUILayout.Button("Выбрать", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Выберите папку для поиска", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                // Преобразуем абсолютный путь в относительный (начиная с Assets)
                if (selected.StartsWith(Application.dataPath))
                {
                    searchFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                }
                else
                {
                    searchFolder = selected;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Найти все Debug.Log в указанной папке"))
        {
            FindAllLogs();
        }

        GUILayout.Space(10);

        // Фильтр по содержимому
        DrawFilter("Фильтр по тексту:");

        GUILayout.Label($"Найдено вызовов: {entries.Count}", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(filter)
            ? entries
            : entries.Where(e =>
                MatchesFilter(e.filePath, filter) ||
                MatchesFilter(e.logText, filter) ||
                MatchesFilter(e.logType, filter)
            ).ToList();

        foreach (var entry in filtered)
        {
            EditorGUILayout.BeginHorizontal();

            // Тип лога с цветом
            Color originalColor = GUI.color;
            switch (entry.logType)
            {
                case "Log": GUI.color = Color.white; break;
                case "Warning": GUI.color = Color.yellow; break;
                case "Error": GUI.color = Color.red; break;
                case "Exception": GUI.color = Color.magenta; break;
            }
            GUILayout.Label(entry.logType, GUILayout.Width(80));
            GUI.color = originalColor;

            EditorGUILayout.LabelField(entry.filePath, GUILayout.MinWidth(200));
            EditorGUILayout.LabelField($"стр. {entry.lineNumber}", GUILayout.Width(60));
            EditorGUILayout.LabelField(entry.logText, GUILayout.MinWidth(150));

            if (GUILayout.Button("Открыть", GUILayout.Width(60)))
            {
                OpenFile(entry.filePath, entry.lineNumber);
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        // Кнопка для очистки списка
        if (entries.Count > 0 && GUILayout.Button("Очистить список"))
        {
            entries.Clear();
        }
    }

    private void FindAllLogs()
    {
        entries.Clear();

        // Проверяем, что папка существует
        string fullFolder = Path.Combine(Application.dataPath, searchFolder.Replace("Assets/", ""));
        if (!Directory.Exists(fullFolder))
        {
            Debug.LogError($"Папка не найдена: {fullFolder}");
            return;
        }

        // Ищем все .cs файлы в папке и подпапках
        var csFiles = Directory.GetFiles(fullFolder, "*.cs", SearchOption.AllDirectories);

        foreach (var file in csFiles)
        {
            // Преобразуем абсолютный путь в относительный (для отображения в редакторе)
            string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');
            ScanFile(relativePath);
        }

        Debug.Log($"Поиск завершён. Найдено вызовов Debug.Log: {entries.Count}");
    }

    private void ScanFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;

        string content = File.ReadAllText(fullPath);

        // Регулярные выражения для разных типов Debug
        var patterns = new List<(string type, string pattern)>();

        if (includeLog)
            patterns.Add(("Log", @"Debug\.Log\s*\(\s*""((?:[^""\\]|\\.)*)"""));
        if (includeWarning)
            patterns.Add(("Warning", @"Debug\.LogWarning\s*\(\s*""((?:[^""\\]|\\.)*)"""));
        if (includeError)
            patterns.Add(("Error", @"Debug\.LogError\s*\(\s*""((?:[^""\\]|\\.)*)"""));
        if (includeException)
            patterns.Add(("Exception", @"Debug\.LogException\s*\(\s*""((?:[^""\\]|\\.)*)"""));

        foreach (var (type, pattern) in patterns)
        {
            var matches = Regex.Matches(content, pattern);
            foreach (Match match in matches)
            {
                string text = match.Groups[1].Value;
                // Можно также перехватывать вызовы с @"" и $"" – но упростим, чтобы не усложнять
                // Если нужно больше вариантов, можно расширить.
                entries.Add(new DebugLogEntry
                {
                    filePath = assetPath,
                    lineNumber = GetLineNumber(content, match.Index),
                    logType = type,
                    logText = text
                });
            }
        }

        // Дополнительно ищем вызовы без строкового литерала (например, с переменной)
        // Это сложнее, для простоты ограничимся строковыми литералами.
        // Можно также добавить поиск Debug.LogFormat и т.д. при желании.
    }

    // Вспомогательный класс для записи
    private class DebugLogEntry
    {
        public string filePath;
        public int lineNumber;
        public string logType;    // "Log", "Warning", "Error", "Exception"
        public string logText;    // содержимое строки
    }
}