using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FormatAllScripts
{
    private const string RootFolder = "Assets/Scripts";
    private static readonly string[] ExcludeFolders = { "/Editor/", "/Plugins/", "/Tests/" };

    [MenuItem("Tools/Format All Scripts")]
    public static void FormatAll()
    {
        string rootFullPath = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));

        if (!Directory.Exists(rootFullPath))
        {
            UnityEngine.Debug.LogError($"Папка '{RootFolder}' не найдена!");
            return;
        }

        string[] csFiles = Directory.GetFiles(rootFullPath, "*.cs", SearchOption.AllDirectories);
        List<string> filesToFormat = new List<string>();

        foreach (string filePath in csFiles)
        {
            bool skip = false;

            foreach (string exclude in ExcludeFolders)
            {
                if (filePath.Contains(exclude))
                {
                    skip = true;
                    break;
                }
            }

            if (!skip)
                filesToFormat.Add(filePath);
        }

        if (filesToFormat.Count == 0)
        {
            UnityEngine.Debug.Log("Нет файлов для форматирования.");
            return;
        }

        bool success = RunFormatter(rootFullPath);

        if (success)
        {
            RemoveTrailingEmptyLines(filesToFormat);
            UnityEngine.Debug.Log($"✅ Отформатировано {filesToFormat.Count} файлов.");
            AssetDatabase.Refresh();
        }
        else
        {
            UnityEngine.Debug.LogError("❌ Ошибка при форматировании.");
        }
    }

    private static bool RunFormatter(string folderPath)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "csharpier",
                Arguments = $"format \"{folderPath}\"",
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    UnityEngine.Debug.Log(output);
                    return true;
                }
                else
                {
                    UnityEngine.Debug.LogError(error);
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Не удалось запустить CSharpier: {ex.Message}");
            return false;
        }
    }

    private static void RemoveTrailingEmptyLines(List<string> filePaths)
    {
        foreach (string file in filePaths)
        {
            string content = File.ReadAllText(file);

            int lastBrace = content.LastIndexOf('}');
            if (lastBrace == -1)
                continue;

            string trimmed = content.Substring(0, lastBrace + 1);

            File.WriteAllText(file, trimmed);
        }
    }
}