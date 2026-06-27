using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Модуль для поиска скриптов с проблемами форматирования.
/// Отображает список проблем для каждого файла в раскрывающемся списке.
/// </summary>
public class CodeFormatterCheckerModule : ToolModuleBase
{
    private string searchFolder = "Assets/Scripts";
    private int problemThreshold = 1;
    private List<FormattingIssueFile> filesWithIssues = new List<FormattingIssueFile>();

    // Для хранения состояния развёрнутости
    private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    public CodeFormatterCheckerModule()
    {
        Name = "Форматирование кода";
    }

    public override void Draw()
    {
        EditorGUILayout.HelpBox(
            "Находит скрипты с нарушениями форматирования:\n" +
            "• неправильные отступы (не кратны 4 пробелам)\n" +
            "• смесь табуляций и пробелов\n" +
            "• пробелы в конце строк\n" +
            "• строки длиннее 120 символов\n\n" +
            "Нажмите на треугольник слева от файла, чтобы увидеть список проблем.",
            MessageType.Info);

        EditorGUILayout.LabelField("Настройки", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Папка поиска:", GUILayout.Width(100));
        searchFolder = EditorGUILayout.TextField(searchFolder);
        if (GUILayout.Button("Выбрать", GUILayout.Width(60)))
        {
            string selected = EditorUtility.OpenFolderPanel("Выберите папку для поиска", "Assets", "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                    searchFolder = "Assets" + selected.Substring(Application.dataPath.Length);
                else
                    searchFolder = selected;
            }
        }
        EditorGUILayout.EndHorizontal();

        problemThreshold = EditorGUILayout.IntSlider("Порог проблем (показывать файлы с ≥)", problemThreshold, 1, 50);

        if (GUILayout.Button("Проверить форматирование"))
        {
            ScanFormatting();
        }

        GUILayout.Space(10);

        DrawFilter("Фильтр по пути:");

        GUILayout.Label($"Найдено файлов с проблемами: {filesWithIssues.Count}", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrEmpty(filter)
            ? filesWithIssues
            : filesWithIssues.Where(f => MatchesFilter(f.filePath, filter)).ToList();

        foreach (var file in filtered)
        {
            if (file.ignored) continue;

            EditorGUILayout.BeginHorizontal();

            // Иконка статуса (количество проблем)
            string icon = file.IssueCount > 20 ? "🔴" : file.IssueCount > 10 ? "🟠" : "🟡";
            EditorGUILayout.LabelField(icon, GUILayout.Width(25));

            // Название файла и количество проблем
            EditorGUILayout.LabelField(file.filePath, GUILayout.MinWidth(250));
            EditorGUILayout.LabelField($"проблем: {file.IssueCount}", GUILayout.Width(80));

            // Кнопка "Открыть"
            if (GUILayout.Button("Открыть", GUILayout.Width(60)))
            {
                OpenFile(file.filePath, 1);
            }

            // Кнопка игнорирования
            if (GUILayout.Button("✔", GUILayout.Width(25)))
            {
                file.ignored = true;
                // Если состояние развёрнутости было, удалим его
                if (foldoutStates.ContainsKey(file.filePath))
                    foldoutStates.Remove(file.filePath);
            }

            EditorGUILayout.EndHorizontal();

            // ---- РАСКРЫВАЮЩИЙСЯ СПИСОК ПРОБЛЕМ ----
            // Получаем или создаём состояние для этого файла
            if (!foldoutStates.ContainsKey(file.filePath))
                foldoutStates[file.filePath] = false;

            // Отображаем foldout с количеством проблем
            string foldoutLabel = $"Подробности ({file.issues.Count} проблем)";
            foldoutStates[file.filePath] = EditorGUILayout.Foldout(foldoutStates[file.filePath], foldoutLabel);

            if (foldoutStates[file.filePath])
            {
                // Отображаем список проблем с отступом
                EditorGUILayout.BeginVertical("box");
                foreach (var problem in file.issues)
                {
                    EditorGUILayout.BeginHorizontal();
                    // Цвет в зависимости от severity
                    Color originalColor = GUI.color;
                    if (problem.severity == 3)
                        GUI.color = Color.red;
                    else if (problem.severity == 2)
                        GUI.color = Color.yellow;
                    else
                        GUI.color = Color.white;

                    string lineInfo = problem.line > 0 ? $"стр. {problem.line}" : "общее";
                    EditorGUILayout.LabelField($"• {lineInfo}: {problem.description}", GUILayout.MinWidth(200));
                    GUI.color = originalColor;

                    // Кнопка перехода к строке (если номер строки указан)
                    if (problem.line > 0 && GUILayout.Button("Перейти", GUILayout.Width(60)))
                    {
                        OpenFile(file.filePath, problem.line);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5); // разделитель между файлами
        }

        GUILayout.EndScrollView();

        if (filesWithIssues.Count > 0 && GUILayout.Button("Очистить список"))
        {
            filesWithIssues.Clear();
            foldoutStates.Clear();
        }
    }

    // ---- МЕТОДЫ СКАНИРОВАНИЯ И АНАЛИЗА (без изменений) ----
    private void ScanFormatting()
    {
        filesWithIssues.Clear();
        foldoutStates.Clear();

        string fullFolder = Path.Combine(Application.dataPath, searchFolder.Replace("Assets/", ""));
        if (!Directory.Exists(fullFolder))
        {
            Debug.LogError($"Папка не найдена: {fullFolder}");
            return;
        }

        var csFiles = Directory.GetFiles(fullFolder, "*.cs", SearchOption.AllDirectories);
        int totalFiles = csFiles.Length;

        for (int i = 0; i < totalFiles; i++)
        {
            string file = csFiles[i];
            string relativePath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');

            if (EditorUtility.DisplayCancelableProgressBar(
                "Проверка форматирования",
                $"Обработка: {relativePath}",
                (float)i / totalFiles))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            var issues = AnalyzeFile(relativePath);
            if (issues.Count >= problemThreshold)
            {
                filesWithIssues.Add(new FormattingIssueFile
                {
                    filePath = relativePath,
                    IssueCount = issues.Count,
                    issues = issues
                });
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Проверка форматирования завершена. Найдено файлов с проблемами: {filesWithIssues.Count}");
    }

    private List<FormattingProblem> AnalyzeFile(string assetPath)
    {
        var problems = new List<FormattingProblem>();
        string fullPath = Path.Combine(Application.dataPath, assetPath.Substring(7));
        if (!File.Exists(fullPath)) return problems;

        string[] lines = File.ReadAllLines(fullPath);
        bool hasTabs = false;
        bool hasSpaces = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int lineNum = i + 1;

            // Пробел в конце строки
            if (line.Length > 0 && line[line.Length - 1] == ' ')
            {
                problems.Add(new FormattingProblem { line = lineNum, description = "Пробел в конце строки", severity = 1 });
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                int idx = 0;
                while (idx < line.Length && char.IsWhiteSpace(line[idx]))
                {
                    if (line[idx] == '\t') hasTabs = true;
                    else if (line[idx] == ' ') hasSpaces = true;
                    idx++;
                }

                // Проверка отступа (только если используются пробелы и нет табов)
                if (idx > 0 && line[idx - 1] == ' ' && !line.Contains("\t"))
                {
                    int spaceCount = idx;
                    if (spaceCount % 4 != 0)
                    {
                        problems.Add(new FormattingProblem
                        {
                            line = lineNum,
                            description = $"Отступ {spaceCount} пробелов (не кратен 4)",
                            severity = 2
                        });
                    }
                }

                // Длина строки
                if (line.Length > 120)
                {
                    problems.Add(new FormattingProblem
                    {
                        line = lineNum,
                        description = $"Длина строки {line.Length} символов (>120)",
                        severity = 1
                    });
                }
            }
        }

        // Общая проблема: смесь табуляций и пробелов
        if (hasTabs && hasSpaces)
        {
            problems.Add(new FormattingProblem
            {
                line = 0,
                description = "Смесь табуляций и пробелов",
                severity = 3
            });
        }

        return problems;
    }

    // ---- ВСПОМОГАТЕЛЬНЫЕ КЛАССЫ ----
    private class FormattingIssueFile
    {
        public string filePath;
        public int IssueCount;
        public List<FormattingProblem> issues;
        public bool ignored;
    }

    private class FormattingProblem
    {
        public int line;          // 0 – общая проблема
        public string description;
        public int severity;      // 1-низкая, 2-средняя, 3-высокая
    }
}