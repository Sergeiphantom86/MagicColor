using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class CompositeToolsWindow : EditorWindow
{
    private List<IToolModule> modules = new List<IToolModule>();
    private int selectedModuleIndex = 0;
    private string[] moduleNames;
    private bool showSettings = false;

    [MenuItem("Tools/Композитные инструменты проверки")]
    public static void ShowWindow()
    {
        GetWindow<CompositeToolsWindow>("Инструменты проверки");
    }

    private void OnEnable()
    {
        // Автоматически найти все типы, реализующие IToolModule
        var moduleTypes = Assembly.GetAssembly(typeof(IToolModule))
            .GetTypes()
            .Where(t => typeof(IToolModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        modules.Clear();
        foreach (var type in moduleTypes)
        {
            try
            {
                var instance = (IToolModule)Activator.CreateInstance(type);
                modules.Add(instance);
            }
            catch (Exception e)
            {
                Debug.LogError($"Не удалось создать модуль {type.Name}: {e.Message}");
            }
        }

        // Сортировка по имени
        modules = modules.OrderBy(m => m.Name).ToList();
        moduleNames = modules.Select(m => m.Name).ToArray();

        // Если есть модули, активируем первый
        if (modules.Count > 0)
            modules[0].OnModuleEnabled();
    }

    private void OnGUI()
    {
        if (modules.Count == 0)
        {
            EditorGUILayout.HelpBox("Не найдено ни одного модуля. Добавьте классы, реализующие IToolModule.", MessageType.Warning);
            return;
        }

        DrawToolbar();

        // Галочка "Показать настройки" (для включения/отключения модулей)
        showSettings = EditorGUILayout.Toggle("Показать настройки модулей", showSettings);
        if (showSettings)
            DrawModuleSettings();

        GUILayout.Space(10);

        // Если текущий модуль включён, отрисовываем его
        var currentModule = modules[selectedModuleIndex];
        if (currentModule.IsEnabled)
        {
            currentModule.Draw();
        }
        else
        {
            EditorGUILayout.HelpBox($"Модуль '{currentModule.Name}' отключён. Включите его в настройках.", MessageType.Info);
        }
    }

    private void DrawToolbar()
    {
        if (moduleNames.Length == 0) return;

        // Вкладки
        int newIndex = GUILayout.Toolbar(selectedModuleIndex, moduleNames);
        if (newIndex != selectedModuleIndex)
        {
            // Деактивировать старый модуль
            modules[selectedModuleIndex].OnModuleDisabled();
            selectedModuleIndex = newIndex;
            // Активировать новый
            modules[selectedModuleIndex].OnModuleEnabled();
        }
    }

    private void DrawModuleSettings()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("Управление модулями", EditorStyles.boldLabel);
        foreach (var module in modules)
        {
            EditorGUILayout.BeginHorizontal();
            bool newState = EditorGUILayout.Toggle(module.Name, module.IsEnabled);
            if (newState != module.IsEnabled)
            {
                module.IsEnabled = newState;
                if (newState)
                    module.OnModuleEnabled();
                else
                    module.OnModuleDisabled();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}