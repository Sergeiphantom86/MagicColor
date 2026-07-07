using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;

public class NamespaceUpdater1 : MonoBehaviour
{
    [MenuItem("Tools/Replace PuzzleEditor Namespace")]
    public static void ReplaceNamespace()
    {
        string folderPath = "Assets/Scripts/PuzzleEditor";

        // Проверяем, существует ли папка
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Папка {folderPath} не найдена!");
            return;
        }

        // Получаем все .cs файлы в папке и подпапках
        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { folderPath });
        int replacedCount = 0;

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!assetPath.EndsWith(".cs")) continue;

            // Читаем содержимое
            string content = File.ReadAllText(assetPath);
            string originalContent = content;

            // Заменяем объявление namespace
            // Вариант 1: "namespace PuzzleEditor" (с пробелом или без)
            // Вариант 2: "namespace PuzzleEditor." для вложенных пространств
            content = Regex.Replace(content,
                @"\bnamespace\s+PuzzleEditor\b",
                "namespace PuzzleResources");

            content = Regex.Replace(content,
                @"\bnamespace\s+PuzzleEditor\.",
                "namespace PuzzleResources.");

            // Если содержимое изменилось – сохраняем
            if (content != originalContent)
            {
                File.WriteAllText(assetPath, content);
                replacedCount++;
                Debug.Log($"Обновлён: {assetPath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"Готово. Обновлено файлов: {replacedCount}");
    }
}