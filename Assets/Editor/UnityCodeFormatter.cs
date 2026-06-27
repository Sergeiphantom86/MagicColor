using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class UnityCodeFormatter
{
    private const string RootFolder = "Assets/Scripts";
    private static readonly string[] ExcludeFolders = { "/Editor/", "/Plugins/", "/Tests/" };

    [MenuItem("Tools/Форматирование кода (Unity Safe)")]
    public static void FormatAll()
    {
        string rootFullPath = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));

        if (!Directory.Exists(rootFullPath))
        {
            Debug.LogError("Scripts folder not found");
            return;
        }

        string[] files = Directory.GetFiles(rootFullPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            if (IsExcluded(file))
                continue;

            string code = File.ReadAllText(file);

            code = NormalizeNewLines(code);
            code = FixBracesAndIndent(code);
            code = RemoveExtraEmptyLines(code);
            code = TrimFile(code);

            File.WriteAllText(file, code);
        }

        AssetDatabase.Refresh();
        Debug.Log("Unity formatter done");
    }

    // -------------------------
    // исключения
    // -------------------------
    private static bool IsExcluded(string path)
    {
        foreach (var exclude in ExcludeFolders)
            if (path.Contains(exclude))
                return true;

        return false;
    }

    // -------------------------
    // нормализация строк
    // -------------------------
    private static string NormalizeNewLines(string text)
    {
        return text.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    // -------------------------
    // основной форматтер (Ctrl+K,D-like)
    // -------------------------
    private static string FixBracesAndIndent(string text)
    {
        var lines = text.Split('\n');
        var result = new List<string>();

        int indent = 0;

        foreach (var raw in lines)
        {
            string line = raw.Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                result.Add("");
                continue;
            }

            if (line == "}")
            {
                indent = Math.Max(0, indent - 1);
            }

            string indentSpaces = new string(' ', indent * 4);
            result.Add(indentSpaces + line);


            if (line.EndsWith("{"))
            {
                indent++;
            }
        }

        return string.Join("\n", result);
    }

    // -------------------------
    // убираем лишние пустые строки
    // -------------------------
    private static string RemoveExtraEmptyLines(string text)
    {
        var lines = text.Split('\n');
        var result = new List<string>();

        bool lastWasEmpty = false;

        foreach (var line in lines)
        {
            bool empty = string.IsNullOrWhiteSpace(line);

            if (empty)
            {
                if (lastWasEmpty)
                    continue;

                lastWasEmpty = true;
                result.Add("");
                continue;
            }

            lastWasEmpty = false;
            result.Add(line);
        }

        return string.Join("\n", result);
    }

    // -------------------------
    // обрезка файла
    // -------------------------
    private static string TrimFile(string text)
    {
        return text.Trim();
    }
}