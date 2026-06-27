using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class NamespaceUpdater
{
    private const string RootFolder = "Assets/Scripts";

    private static readonly string[] ExcludeFolders =
    {
        "/Editor/",
        "/Plugins/",
        "/Tests/",
    };

    //[MenuItem("Обновить пространства имен")]
    public static void UpdateAllNamespaces()
    {
        string rootPath = GetRootPath();

        if (!Directory.Exists(rootPath))
        {
            Debug.LogError($"Папка '{RootFolder}' не найдена!");
            return;
        }

        string[] files = Directory.GetFiles(rootPath, "*.cs", SearchOption.AllDirectories);

        int processed = 0;

        foreach (string file in files)
        {
            if (IsExcluded(file))
                continue;

            if (FixNamespace(file, rootPath))
                processed++;
        }

        Debug.Log($"✅ Исправлено namespace в {processed} файлах из {files.Length}.");

        AssetDatabase.Refresh();
    }

    private static string GetRootPath()
    {
        return Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));
    }

    private static bool IsExcluded(string filePath)
    {
        foreach (string folder in ExcludeFolders)
        {
            if (filePath.Replace('\\', '/').Contains(folder))
                return true;
        }

        return false;
    }

    private static bool FixNamespace(string filePath, string rootPath)
    {
        string relativePath = filePath
            .Substring(rootPath.Length)
            .TrimStart('/', '\\')
            .Replace('\\', '/');

        string folder = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? string.Empty;

        string newNamespace = string.IsNullOrEmpty(folder)
            ? "Scripts"
            : folder.Replace('/', '.');

        string content = File.ReadAllText(filePath);

        Match match = Regex.Match(content, @"namespace\s+([^\s{;]+)");

        if (match.Success)
            return ReplaceNamespace(filePath, content, match.Groups[1].Value, newNamespace);

        return InsertNamespace(filePath, content, newNamespace);
    }

    private static bool ReplaceNamespace(
        string filePath,
        string content,
        string oldNamespace,
        string newNamespace)
    {
        if (oldNamespace == newNamespace)
            return false;

        content = Regex.Replace(
            content,
            @"namespace\s+" + Regex.Escape(oldNamespace),
            $"namespace {newNamespace}");

        File.WriteAllText(filePath, content);

        return true;
    }

    private static bool InsertNamespace(
        string filePath,
        string content,
        string newNamespace)
    {
        int insertIndex = GetInsertIndex(content);

        content = content.Insert(
            insertIndex,
            $"namespace {newNamespace}\n{{\n");

        content += "\n}";

        File.WriteAllText(filePath, content);

        return true;
    }

    private static int GetInsertIndex(string content)
    {
        int lastUsing = content.LastIndexOf("using ", StringComparison.Ordinal);

        if (lastUsing < 0)
            return 0;

        int endLine = content.IndexOf('\n', lastUsing);

        return endLine < 0
            ? content.Length
            : endLine + 1;
    }
}