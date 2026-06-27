using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class AutoFixEventNaming
{
    private const string RootFolder = "Assets/Scripts";
    private static readonly string[] ExcludeFolders = { "/Editor/", "/Plugins/", "/Tests/" };

    private static readonly HashSet<string> IgnoredEventNames = new HashSet<string>
    {
        "OnClick", "OnPointerClick", "OnPointerDown", "OnPointerUp",
        "OnPointerEnter", "OnPointerExit", "OnBeginDrag", "OnDrag",
        "OnEndDrag", "OnDrop", "OnScroll",
        "OnDestroy", "OnEnable", "OnDisable", "OnApplicationPause",
        "OnApplicationFocus", "OnApplicationQuit", "OnValidate", "OnGUI",
        "OnDrawGizmos", "OnRenderObject", "OnWillRenderObject",
        "OnPreCull", "OnPreRender", "OnPostRender",
        "OnBecameVisible", "OnBecameInvisible",
        "OnControllerColliderHit", "OnJointBreak", "OnParticleCollision",
        "OnTransformChildrenChanged", "OnTransformParentChanged",
        "OnAnimatorMove", "OnAnimatorIK",
        "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit",
        "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit",
        "OnCollisionEnter2D", "OnCollisionStay2D", "OnCollisionExit2D",
        "OnTriggerEnter2D", "OnTriggerStay2D", "OnTriggerExit2D"
    };

    [MenuItem("Tools/Автоматическое исправление именования событий")]
    public static void FixAll()
    {
        if (!EditorUtility.DisplayDialog("Warning",
                "Tool will rename events (remove On) and handler methods (add On).\n\n" +
                "Backups will be created.\n\n" +
                "Make sure code is committed! Continue?",
                "Continue", "Cancel"))
            return;

        string rootFullPath = Path.Combine(Application.dataPath, RootFolder.Substring("Assets/".Length));
        if (!Directory.Exists(rootFullPath))
        {
            Debug.LogError($"Folder '{RootFolder}' not found!");
            return;
        }

        string backupFolder = "Backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupFullPath = Path.Combine(rootFullPath, backupFolder);
        Directory.CreateDirectory(backupFullPath);

        string[] csFiles = Directory.GetFiles(rootFullPath, "*.cs", SearchOption.AllDirectories)
            .Where(f => !ExcludeFolders.Any(e => f.Replace('\\', '/').Contains(e)))
            .ToArray();

        if (csFiles.Length == 0)
        {
            Debug.Log("No files to process.");
            return;
        }

        var eventRenames = new Dictionary<string, string>();
        var methodRenames = new Dictionary<string, string>();

        foreach (string file in csFiles)
        {
            string content = File.ReadAllText(file);

            string eventPattern = @"\bevent\s+[\w<>.]+\s+On(\w+)\s*[;=({]";
            foreach (Match m in Regex.Matches(content, eventPattern))
            {
                string oldName = "On" + m.Groups[1].Value;
                if (IgnoredEventNames.Contains(oldName))
                    continue;
                string newName = m.Groups[1].Value;
                AddRename(eventRenames, file, oldName, newName);
            }

            string handlerPattern = @"(?:\+=|-=|\.AddListener\s*\()\s*(\w+)\s*[;)]";
            foreach (Match m in Regex.Matches(content, handlerPattern))
            {
                string methodName = m.Groups[1].Value;
                if (string.IsNullOrEmpty(methodName) || methodName.Contains("."))
                    continue;
                if (methodName.StartsWith("On") || IsUnityMethod(methodName))
                    continue;

                if (!IsMethodPrivateAndNotOverride(content, methodName))
                    continue;

                string newName = "On" + methodName;

                if (HasFieldOrProperty(content, newName))
                {
                    Debug.LogWarning($"In file {Path.GetFileName(file)} skipped method {methodName} – field/property named {newName} already exists");
                    continue;
                }

                AddRename(methodRenames, file, methodName, newName);
            }
        }

        if (eventRenames.Count == 0 && methodRenames.Count == 0)
        {
            Debug.Log("No violations found.");
            return;
        }

        Debug.Log($"Found event renames: {eventRenames.Count}, method renames: {methodRenames.Count}");

        foreach (string file in csFiles)
        {
            string relativePath = GetRelativePath(file, rootFullPath);
            string backupFilePath = Path.Combine(backupFullPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(backupFilePath));
            File.Copy(file, backupFilePath, true);
        }

        ApplyRenames(csFiles, eventRenames);
        ApplyRenames(csFiles, methodRenames);

        AssetDatabase.Refresh();
        Debug.Log($"Auto fix completed. Check changes in backup folder: {backupFolder}");
    }

    private static void AddRename(Dictionary<string, string> dict, string file, string oldName, string newName)
    {
        if (!dict.ContainsKey(file))
            dict[file] = oldName + "|" + newName;
        else
            dict[file] += ";" + oldName + "|" + newName;
    }

    private static void ApplyRenames(string[] allFiles, Dictionary<string, string> renames)
    {
        if (renames.Count == 0) return;
        foreach (var kvp in renames)
        {
            string file = kvp.Key;
            string content = File.ReadAllText(file);
            string[] pairs = kvp.Value.Split(';');
            foreach (string pair in pairs)
            {
                if (string.IsNullOrEmpty(pair)) continue;
                string[] parts = pair.Split('|');
                string oldName = parts[0];
                string newName = parts[1];
                content = Regex.Replace(content, @"\b" + Regex.Escape(oldName) + @"\b", newName);
            }
            File.WriteAllText(file, content);
        }
    }

    private static bool IsUnityMethod(string name)
    {
        string[] unity = {
            "Start", "Update", "FixedUpdate", "LateUpdate", "Awake",
            "OnDestroy", "OnEnable", "OnDisable", "OnGUI",
            "OnApplicationQuit", "OnValidate", "Reset",
            "OnTriggerEnter", "OnTriggerStay", "OnTriggerExit",
            "OnCollisionEnter", "OnCollisionStay", "OnCollisionExit"
        };
        return Array.Exists(unity, m => m == name);
    }

    private static bool IsMethodPrivateAndNotOverride(string content, string methodName)
    {
        string pattern = @"\b(void|[\w<>]+)\s+" + Regex.Escape(methodName) + @"\s*\([^)]*\)\s*[({]";
        var match = Regex.Match(content, pattern);
        if (!match.Success) return false;

        int start = match.Index;
        string before = content.Substring(0, start);

        if (Regex.IsMatch(before, @"\boverride\s+$", RegexOptions.Multiline))
            return false;

        if (Regex.IsMatch(before, @"\b(public|protected|internal)\s+$", RegexOptions.Multiline))
            return false;

        return true;
    }

    private static bool HasFieldOrProperty(string content, string name)
    {
        string pattern = @"\b(private|public|protected|internal)?\s+(?:\w+\s+)*" + Regex.Escape(name) + @"\s*[=;({]";
        return Regex.IsMatch(content, pattern);
    }

    private static string GetRelativePath(string fullPath, string rootPath)
    {
        if (fullPath.StartsWith(rootPath))
            return fullPath.Substring(rootPath.Length + 1);
        return fullPath;
    }
}