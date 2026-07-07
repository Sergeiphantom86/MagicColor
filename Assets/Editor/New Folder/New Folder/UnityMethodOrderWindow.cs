using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityMethodOrderWindow : EditorWindow
{
    private Vector2 _scroll;

    private List<UnityMethodOrderViolation> _violations = new();

    // Скрытые нарушения (только до закрытия Unity)
    private readonly HashSet<string> _hiddenViolations = new();

    [MenuItem("Tools/Code Style/Check Unity Method Order")]
    private static void Open()
    {
        GetWindow<UnityMethodOrderWindow>("Unity Method Order");
    }

    private void OnGUI()
    {
        GUILayout.Space(5);

        if (GUILayout.Button("Сканировать", GUILayout.Height(35)))
        {
            _violations = UnityMethodOrderChecker.CheckFolder("Assets/Scripts");
        }

        GUILayout.Space(10);

        int visibleCount = 0;

        foreach (var violation in _violations)
        {
            if (_hiddenViolations.Contains(violation.Id) == false)
                visibleCount++;
        }

        EditorGUILayout.LabelField(
            $"Найдено нарушений: {visibleCount}",
            EditorStyles.boldLabel);

        GUILayout.Space(5);

        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        foreach (var violation in _violations)
        {
            if (_hiddenViolations.Contains(violation.Id))
                continue;

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField(
                violation.AssetPath,
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                violation.Description,
                MessageType.Warning);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Перейти"))
            {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(violation.AssetPath);

                if (script != null)
                {
                    AssetDatabase.OpenAsset(script);
                    EditorGUIUtility.PingObject(script);
                }
            }

            if (GUILayout.Button("Скрыть", GUILayout.Width(80)))
            {
                _hiddenViolations.Add(violation.Id);
                Repaint();
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(3);
        }

        EditorGUILayout.EndScrollView();
    }
}