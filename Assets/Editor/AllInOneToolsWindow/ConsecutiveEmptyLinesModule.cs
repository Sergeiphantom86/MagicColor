using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Модуль для поиска двух и более подряд идущих пустых строк в скриптах.
/// </summary>
public class ConsecutiveEmptyLinesModule : ToolModuleBase
{
    private List<EmptyLinesIssue> issues = new List<EmptyLinesIssue>();

    // Настройки
    private int minEmptyLines = 2; // минимальное количество пустых строк для обнаружения
    private string searchFolder = "Assets/Scripts";

    public ConsecutiveEmptyLinesModule()
    {
        Name = "Пустые строки";
    }

    public override void Draw()
    {
        EditorGUILayout.HelpBox(
            "Находит участки с двумя и более подряд идущими пустыми строками в скриптах.",
            MessageType.Info);

        // Настройки
        EditorGUILayout.LabelField("Настройки", EditorStyles.boldLabel);
        minEmptyLines = EditorGUILayout.IntSlider("Минимальное количество пустых строк", minEmptyLines, 2, 5);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Папка поиска:", GUILayout.Width(100));
        searchFolder = EditorGUILayout.TextField(searchFolder);
        if (GUILayout.Button("Выбрать", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Выберите папку для поиска", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
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

        if (GUILayout.Button($"Сканировать на {minEmptyLines} и более пустых строк"))
        {
            ScanAllScripts();
        }

        GUILayout.Space(10);

        DrawFilter("Фильтр по пути:");

        GUILayout.Label($"Найдено проблем: {issues.Count}", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(filter)
            ? issues
            : issues.Where(i => MatchesFilter(i.filePath, filter)).ToList();

        foreach (var issue in filtered)
        {
            if (issue.ignored) continue;

            EditorGUILayout.BeginHorizontal();

            // Иконка и информация
            GUILayout.Label("⚠️", GUILayout.Width(25));
            EditorGUILayout.LabelField(issue.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField($"стр. {issue.startLine} - {issue.endLine}", GUILayout.Width(120));
            EditorGUILayout.LabelField($"пустых строк: {issue.emptyLineCount}", GUILayout.Width(100));

            // Кнопка открытия файла
            if (GUILayout.Button("Открыть", GUILayout.Width(60)))
            {
                OpenFile(issue.filePath, issue.startLine);
            }

            // Кнопка "забыть"
            if (GUILayout.Button("✔", GUILayout.Width(25)))
            {
                issue.ignored = true;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        if (issues.Count > 0 && GUILayout.Button("Очистить список"))
        {
            issues.Clear();
        }
    }

    private void ScanAllScripts()
    {
        issues.Clear();

        string fullFolder = Path.Combine(Application.dataPath, searchFolder.Replace("Assets/", ""));
        if (!Directory.Exists(fullFolder))
        {
            Debug.LogError($"Папка не найдена: {fullFolder}");
            return;
        }

        var csFiles = Directory.GetFiles(fullFolder, "*.cs", SearchOption.AllDirectories);

        foreach (var file in csFiles)
        {
            string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');
            ScanFile(relativePath);
        }

        Debug.Log($"Сканирование завершено. Найдено проблем: {issues.Count}");
    }

    private void ScanFile(string assetPath)
    {
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return;

        string[] lines = File.ReadAllLines(fullPath);

        int consecutiveEmpty = 0;
        int startLine = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            bool isEmpty = string.IsNullOrWhiteSpace(lines[i]);

            if (isEmpty)
            {
                if (consecutiveEmpty == 0)
                    startLine = i + 1; // номер строки с первой пустой
                consecutiveEmpty++;
            }
            else
            {
                if (consecutiveEmpty >= minEmptyLines)
                {
                    issues.Add(new EmptyLinesIssue
                    {
                        filePath = assetPath,
                        startLine = startLine,
                        endLine = i, // последняя пустая строка (i - 1, но удобнее)
                        emptyLineCount = consecutiveEmpty,
                        // Можно запомнить строки для превью, но для простоты пропустим
                    });
                }
                consecutiveEmpty = 0;
                startLine = -1;
            }
        }

        // Проверка в конце файла
        if (consecutiveEmpty >= minEmptyLines)
        {
            issues.Add(new EmptyLinesIssue
            {
                filePath = assetPath,
                startLine = startLine,
                endLine = lines.Length,
                emptyLineCount = consecutiveEmpty,
            });
        }
    }

    private class EmptyLinesIssue
    {
        public string filePath;
        public int startLine;
        public int endLine;
        public int emptyLineCount;
        public bool ignored;
    }
}