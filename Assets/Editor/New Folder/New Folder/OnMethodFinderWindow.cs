using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorTools
{
    public class OnMethodFinderWindow : EditorWindow
    {
        private Vector2 _scrollPosition;
        private List<MethodEntry> _methods = new List<MethodEntry>();
        private bool _isScanning;

        // Игнорируемые записи (ключ = "путь_к_файлу|имя_метода")
        private HashSet<string> _ignoredEntries = new HashSet<string>();
        private const string IgnoreListKey = "OnMethodFinder_IgnoreList";

        // Стандартные методы Unity, которые мы пропускаем (регистронезависимо)
        private static readonly HashSet<string> UnityMethods = new HashSet<string>
        {
            "OnEnable", "OnDisable", "OnDestroy", "OnApplicationQuit",
            "OnApplicationFocus", "OnApplicationPause", "OnGUI",
            "OnAnimatorMove", "OnAnimatorIK", "OnCollisionEnter",
            "OnCollisionStay", "OnCollisionExit", "OnTriggerEnter",
            "OnTriggerStay", "OnTriggerExit", "OnControllerColliderHit",
            "OnJointBreak", "OnParticleCollision", "OnTransformChildrenChanged",
            "OnTransformParentChanged", "OnDrawGizmos", "OnDrawGizmosSelected",
            "OnValidate", "OnReset", "OnPreCull", "OnPreRender", "OnPostRender",
            "OnRenderObject", "OnWillRenderObject", "OnBecameVisible",
            "OnBecameInvisible", "OnAudioFilterRead", "OnRenderImage"
        };

        [MenuItem("Window/Find `On*` Methods")]
        public static void ShowWindow()
        {
            var window = GetWindow<OnMethodFinderWindow>();
            window.titleContent = new GUIContent("On* Methods");
            window.Show();
        }

        private void OnEnable()
        {
            LoadIgnoredList();
        }

        private void OnGUI()
        {
            // Верхняя панель с кнопками
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Scan Assets/Scripts", GUILayout.Width(150), GUILayout.Height(25)))
            {
                ScanScripts();
            }
            if (GUILayout.Button("Clear Ignored", GUILayout.Width(120), GUILayout.Height(25)))
            {
                ClearIgnoredList();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"Ignored: {_ignoredEntries.Count}", GUILayout.Width(80));
            EditorGUILayout.EndHorizontal();

            if (_isScanning)
            {
                EditorGUILayout.HelpBox("Scanning...", MessageType.Info);
                return;
            }

            if (_methods.Count == 0)
            {
                EditorGUILayout.HelpBox("No methods found. Press 'Scan' to start.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            foreach (var entry in _methods)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{entry.MethodName}  (in {entry.FileName})", EditorStyles.label);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Go to", GUILayout.Width(60)))
                {
                    OpenFileAtLine(entry.FilePath, entry.LineNumber);
                }
                if (GUILayout.Button("Ignore", GUILayout.Width(60)))
                {
                    IgnoreMethod(entry);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }

        private void ScanScripts()
        {
            _isScanning = true;
            _methods.Clear();

            try
            {
                string scriptsPath = Path.Combine(Application.dataPath, "Scripts");
                if (!Directory.Exists(scriptsPath))
                {
                    Debug.LogWarning($"Папка {scriptsPath} не найдена. Проверьте путь.");
                    _isScanning = false;
                    return;
                }

                string[] csFiles = Directory.GetFiles(scriptsPath, "*.cs", SearchOption.AllDirectories);

                foreach (string filePath in csFiles)
                {
                    string relativePath = GetRelativeAssetPath(filePath);
                    if (string.IsNullOrEmpty(relativePath)) continue;

                    try
                    {
                        string content = File.ReadAllText(filePath);
                        FindOnMethods(content, relativePath, filePath);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Ошибка при обработке файла {relativePath}: {ex.Message}");
                    }
                }

                Debug.Log($"Найдено {_methods.Count} методов с префиксом 'On' (исключая стандартные Unity).");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Ошибка при сканировании: {ex.Message}");
            }
            finally
            {
                _isScanning = false;
                Repaint();
            }
        }

        private void FindOnMethods(string content, string relativePath, string fullPath)
        {
            string normalized = content.Replace("\r\n", "\n");

            var methodRegex = new Regex(
                @"^\s*(?:public|private|protected|internal|static)\s+\S+\s+(On\w+)\s*\(",
                RegexOptions.Multiline | RegexOptions.IgnoreCase
            );

            var matches = methodRegex.Matches(normalized);

            foreach (Match match in matches)
            {
                string methodName = match.Groups[1].Value;

                // Пропускаем стандартные методы Unity
                if (UnityMethods.Contains(methodName))
                    continue;

                string entryKey = $"{relativePath}|{methodName}";
                // Пропускаем, если метод в игнор-листе
                if (_ignoredEntries.Contains(entryKey))
                    continue;

                int lineNumber = normalized.Substring(0, match.Index).Split('\n').Length;

                _methods.Add(new MethodEntry
                {
                    MethodName = methodName,
                    FileName = Path.GetFileName(relativePath),
                    FilePath = relativePath,
                    LineNumber = lineNumber
                });
            }
        }

        private string GetRelativeAssetPath(string fullPath)
        {
            string dataPath = Application.dataPath;
            if (fullPath.StartsWith(dataPath))
            {
                return "Assets" + fullPath.Substring(dataPath.Length);
            }
            return null;
        }

        private void OpenFileAtLine(string assetPath, int lineNumber)
        {
            string fullPath = Path.Combine(Application.dataPath, assetPath.Substring("Assets/".Length));
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Файл не найден: {fullPath}");
                return;
            }

            InternalEditorUtility.OpenFileAtLineExternal(fullPath, lineNumber);
        }

        // ---------- Игнорирование ----------
        private void IgnoreMethod(MethodEntry entry)
        {
            string key = $"{entry.FilePath}|{entry.MethodName}";
            if (!_ignoredEntries.Contains(key))
            {
                _ignoredEntries.Add(key);
                SaveIgnoredList();
                // Удаляем из текущего списка
                _methods.Remove(entry);
                Repaint();
            }
        }

        private void ClearIgnoredList()
        {
            _ignoredEntries.Clear();
            SaveIgnoredList();
            // После очистки можно заново отсканировать, чтобы вернуть все методы
            ScanScripts();
        }

        private void LoadIgnoredList()
        {
            _ignoredEntries.Clear();
            string serialized = EditorPrefs.GetString(IgnoreListKey, "");
            if (string.IsNullOrEmpty(serialized)) return;

            string[] entries = serialized.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (string entry in entries)
            {
                _ignoredEntries.Add(entry);
            }
        }

        private void SaveIgnoredList()
        {
            string serialized = string.Join(";", _ignoredEntries);
            EditorPrefs.SetString(IgnoreListKey, serialized);
        }

        // ---------- Вложенный класс ----------
        private class MethodEntry
        {
            public string MethodName;
            public string FileName;
            public string FilePath;
            public int LineNumber;
        }
    }
}