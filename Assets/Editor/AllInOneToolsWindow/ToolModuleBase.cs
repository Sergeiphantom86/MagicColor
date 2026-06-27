using UnityEngine;
using UnityEditor;

public interface IToolModule
{
    string Name { get; }                 // Название модуля (для вкладки)
    bool IsEnabled { get; set; }        // Включён ли модуль
    void Draw();                        // Отрисовка интерфейса модуля
    void OnModuleEnabled();             // Вызывается, когда модуль становится активным (опционально)
    void OnModuleDisabled();            // Вызывается, когда модуль отключается
}

public abstract class ToolModuleBase : IToolModule
{
    public string Name { get; protected set; }
    public bool IsEnabled { get; set; } = true;

    protected Vector2 scrollPos;
    protected string filter = "";

    public virtual void OnModuleEnabled() { }
    public virtual void OnModuleDisabled() { }

    public abstract void Draw();

    /// <summary>
    /// Вспомогательный метод для фильтрации списка по полю пути/имени.
    /// </summary>
    protected bool MatchesFilter(string text, string filter)
    {
        if (string.IsNullOrEmpty(filter)) return true;
        return text.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>
    /// Стандартный виджет фильтра.
    /// </summary>
    protected void DrawFilter(string label = "Фильтр:")
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(label, GUILayout.Width(50));
        filter = EditorGUILayout.TextField(filter);
        if (GUILayout.Button("Очистить", GUILayout.Width(70)))
            filter = "";
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Открыть файл по пути и номеру строки.
    /// </summary>
    protected void OpenFile(string assetPath, int line)
    {
        string fullPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, assetPath.Substring(7)));
        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullPath, line);
    }

    /// <summary>
    /// Получить номер строки по индексу.
    /// </summary>
    protected int GetLineNumber(string content, int index)
    {
        int line = 1;
        for (int i = 0; i < index && i < content.Length; i++)
            if (content[i] == '\n') line++;
        return line;
    }
}