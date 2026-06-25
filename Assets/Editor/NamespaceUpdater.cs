using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class NamespaceUpdater
{
    // Путь к папке со скриптами (относительно Assets) 11111111111
    private const string RootFolder = "Assets/Scripts";
    private static readonly string[] ExcludeFolders = { "/Editor/", "/Plugins/", "/Tests/" };

    [MenuItem("Tools/Update Namespaces (fix)")]
    public static void UpdateAllNamespaces()
    {
        string rootFullPath = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));
        if (!Directory.Exists(rootFullPath))
        {
            Debug.LogError($"Папка '{RootFolder}' не найдена!");
            return;
        }

        string[] csFiles = Directory.GetFiles(rootFullPath, "*.cs", SearchOption.AllDirectories);
        int processed = 0;

        foreach (string filePath in csFiles)
        {
            bool skip = false;
            foreach (string exclude in ExcludeFolders)
                if (filePath.Contains(exclude)) { skip = true; break; }
            if (skip) continue;

            if (FixNamespace(filePath))
                processed++;
        }

        Debug.Log($"✅ Исправлено namespace в {processed} файлах из {csFiles.Length}.");
        AssetDatabase.Refresh();
    }

    private static bool FixNamespace(string filePath)
    {
        // Получаем относительный путь от корня Scripts
        string rootFull = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));
        string relativePath = filePath.Substring(rootFull.Length)
                                    .TrimStart('/', '\\')
                                    .Replace('\\', '/'); // унифицируем разделители

        // Берём только папки (без имени файла)
        string folderPath = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? "";
        // Преобразуем слэши в точки
        string newNamespace = folderPath.Replace('/', '.').Replace('\\', '.');
        // Если файл в корне Scripts – пространство имён будет пустым, но лучше дать "Scripts"
        if (string.IsNullOrEmpty(newNamespace))
            newNamespace = "Scripts";

        string content = File.ReadAllText(filePath);

        // Ищем существующее объявление namespace
        var nsMatch = Regex.Match(content, @"namespace\s+([^\s{;]+)");
        if (nsMatch.Success)
        {
            string oldNs = nsMatch.Groups[1].Value;
            // Если уже правильное – пропускаем
            if (oldNs == newNamespace)
                return false;

            // Заменяем старое на новое (учитываем, что может быть с точками или слешами)
            content = Regex.Replace(content, @"namespace\s+" + Regex.Escape(oldNs), $"namespace {newNamespace}");
            File.WriteAllText(filePath, content);
            return true;
        }
        else
        {
            // Вставляем namespace после всех using
            int insertIdx = content.LastIndexOf("using ", StringComparison.Ordinal);
            if (insertIdx >= 0)
            {
                int endLine = content.IndexOf('\n', insertIdx);
                if (endLine == -1) endLine = insertIdx;
                insertIdx = endLine + 1;
            }
            else
            {
                insertIdx = 0;
            }

            string newContent = content.Insert(insertIdx, $"namespace {newNamespace}\n{{\n") + "\n}";
            File.WriteAllText(filePath, newContent);
            return true;
        }
    }
}