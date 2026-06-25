using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class CheckEventNaming
{
    private const string RootFolder = "Assets/Scripts";
    private static readonly string[] ExcludeFolders = { "/Editor/", "/Plugins/", "/Tests/" };

    [MenuItem("Tools/Check Event Naming")]
    public static void CheckAllScripts()
    {
        string rootFullPath = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));

        if (!Directory.Exists(rootFullPath))
        {
            Debug.LogError($"Папка '{RootFolder}' не найдена!");
            return;
        }

        string[] csFiles = Directory.GetFiles(rootFullPath, "*.cs", SearchOption.AllDirectories);
        List<string> filesToCheck = new List<string>();

        foreach (string filePath in csFiles)
        {
            bool skip = false;
            foreach (string exclude in ExcludeFolders)
            {
                if (filePath.Replace('\\', '/').Contains(exclude))
                {
                    skip = true;
                    break;
                }
            }
            if (!skip)
                filesToCheck.Add(filePath);
        }

        if (filesToCheck.Count == 0)
        {
            Debug.Log("Нет файлов для проверки.");
            return;
        }

        List<string> violations = new List<string>();

        foreach (string file in filesToCheck)
        {
            AnalyzeFile(file, violations);
        }

        if (violations.Count == 0)
        {
            Debug.Log("✅ Нарушений не найдено. Все события и обработчики именованы корректно.");
        }
        else
        {
            Debug.LogError($"❌ Найдено {violations.Count} нарушений:\n" + string.Join("\n", violations));
        }
    }

    private static void AnalyzeFile(string filePath, List<string> violations)
    {
        string content = File.ReadAllText(filePath);
        string[] lines = File.ReadAllLines(filePath);
        string fileName = Path.GetFileName(filePath);

        // 1. Проверка объявлений событий (не должны начинаться с On)
        string eventPattern = @"\bevent\s+[\w<>.]+\s+(\w+)\s*[;=({]";
        MatchCollection eventMatches = Regex.Matches(content, eventPattern);
        foreach (Match match in eventMatches)
        {
            string eventName = match.Groups[1].Value;
            if (eventName.StartsWith("On"))
            {
                int lineNumber = GetLineNumber(content, match.Index);
                violations.Add($"{fileName} (line {lineNumber}): Событие '{eventName}' не должно иметь префикс 'On'.");
            }
        }

        // 2. Проверка методов-обработчиков (используемых в подписках) – должны начинаться с On
        // Ищем шаблоны: += MethodName;   -= MethodName;   .AddListener(MethodName)
        // Также учтём возможные пробелы и переносы строк (упрощённо)
        string handlerPattern = @"(?:\+=|-=|\.AddListener\s*\()\s*(\w+)\s*[;)]";
        MatchCollection handlerMatches = Regex.Matches(content, handlerPattern);
        foreach (Match match in handlerMatches)
        {
            string methodName = match.Groups[1].Value;
            // Игнорируем лямбды или анонимные методы (они не будут идентификаторами)
            if (methodName == "delegate" || methodName == "()" || string.IsNullOrEmpty(methodName))
                continue;
            if (!methodName.StartsWith("On"))
            {
                int lineNumber = GetLineNumber(content, match.Index);
                violations.Add($"{fileName} (line {lineNumber}): Метод-обработчик '{methodName}' должен иметь префикс 'On'.");
            }
        }
    }

    private static int GetLineNumber(string text, int index)
    {
        int line = 1;
        for (int i = 0; i < index; i++)
        {
            if (text[i] == '\n')
                line++;
        }
        return line;
    }
}