using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FixClosingBracketFormat
{
    private const string RootFolder = "Assets/Scripts";

    [MenuItem("Tools/Исправлен формат закрывающей скобки")]
    public static void Fix()
    {
        string targetFolder = Path.Combine(Application.dataPath, RootFolder.Replace("Assets/", ""));

        if (!Directory.Exists(targetFolder))
        {
            Debug.LogWarning($"Папка не найдена: {targetFolder}. Проверьте путь в константе RootFolder.");
            return;
        }

        string[] files = Directory.GetFiles(targetFolder, "*.cs", SearchOption.AllDirectories);
        int fixedCount = 0;

        foreach (string file in files)
        {
            var lines = new List<string>(File.ReadAllLines(file));
            bool changed = false;

            for (int i = 0; i < lines.Count; i++)
            {
                string trimmed = lines[i].TrimStart();

                if (trimmed.StartsWith(")") && trimmed.TrimEnd() == ")")
                {
                    if (i > 0)
                    {
                        string prevLine = lines[i - 1];
                        string prevTrimmed = prevLine.TrimEnd();

                        if (!prevTrimmed.EndsWith(","))
                        {
                            lines[i - 1] = prevTrimmed + ")";
                            lines.RemoveAt(i);
                            changed = true;
                            i--;
                        }
                    }
                }
            }

            if (changed)
            {
                File.WriteAllLines(file, lines);
                fixedCount++;
                Debug.Log($"Исправлено: {file}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Готово. Исправлено файлов: {fixedCount} в папке {RootFolder}");
    }
}