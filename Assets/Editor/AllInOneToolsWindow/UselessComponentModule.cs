using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UselessComponentModule : ToolModuleBase
{
    private bool checkFieldValues = true;
    private List<FoundComponent> foundComponents = new List<FoundComponent>();

    public UselessComponentModule()
    {
        Name = "Бесполезные компоненты";
    }

    public override void Draw()
    {
        checkFieldValues = EditorGUILayout.Toggle("Проверять значения полей (если все по умолчанию – считать бесполезным)", checkFieldValues);

        if (GUILayout.Button("Начать поиск в текущей сцене"))
        {
            FindUselessComponents();
        }

        GUILayout.Space(10);
        DrawFilter();

        GUILayout.Label($"Найдено: {foundComponents.Count}", EditorStyles.boldLabel);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var filtered = string.IsNullOrWhiteSpace(filter)
            ? foundComponents
            : foundComponents.Where(item =>
                MatchesFilter(item.componentName, filter) ||
                MatchesFilter(item.gameObjectPath, filter)
            ).ToList();

        foreach (var item in filtered)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(item.componentName, GUILayout.Width(200));
            EditorGUILayout.LabelField(item.gameObjectPath, GUILayout.Width(300));
            if (GUILayout.Button("Выбрать", GUILayout.Width(60)))
            {
                Selection.activeGameObject = item.gameObject;
                EditorGUIUtility.PingObject(item.gameObject);
            }
            if (GUILayout.Button("Удалить", GUILayout.Width(120)))
            {
                RemoveComponent(item);
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        if (foundComponents.Count > 0 && GUILayout.Button("Удалить все найденные"))
        {
            foreach (var item in foundComponents.ToList())
                RemoveComponent(item);
            foundComponents.Clear();
        }
    }

    private void FindUselessComponents()
    {
        foundComponents.Clear();
        Scene currentScene = SceneManager.GetActiveScene();
        if (!currentScene.isLoaded)
        {
            Debug.LogWarning("Активная сцена не загружена.");
            return;
        }

        var allComponents = new List<Component>();
        foreach (var root in currentScene.GetRootGameObjects())
            allComponents.AddRange(root.GetComponentsInChildren<Component>(true));

        var monoBehaviours = allComponents.OfType<MonoBehaviour>().Where(c => c != null).ToList();

        var allReferences = new HashSet<Component>();
        foreach (var comp in monoBehaviours)
        {
            var fields = comp.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
                            && typeof(Component).IsAssignableFrom(f.FieldType));
            foreach (var field in fields)
            {
                var value = field.GetValue(comp) as Component;
                if (value != null) allReferences.Add(value);
                if (field.FieldType.IsArray)
                {
                    var arr = field.GetValue(comp) as Component[];
                    if (arr != null)
                        foreach (var item in arr) if (item != null) allReferences.Add(item);
                }
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var list = field.GetValue(comp) as System.Collections.IList;
                    if (list != null)
                        foreach (var item in list) if (item is Component c && c != null) allReferences.Add(c);
                }
            }
        }

        foreach (var comp in monoBehaviours)
        {
            if (IsUseless(comp, allReferences))
            {
                foundComponents.Add(new FoundComponent
                {
                    gameObject = comp.gameObject,
                    component = comp,
                    componentName = comp.GetType().Name,
                    gameObjectPath = GetPath(comp.gameObject)
                });
            }
        }

        Debug.Log($"Найдено {foundComponents.Count} бесполезных компонентов в сцене '{currentScene.name}'.");
    }

    private bool IsUseless(MonoBehaviour comp, HashSet<Component> allReferences)
    {
        var type = comp.GetType();
        if (allReferences.Contains(comp)) return false;

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                         .Where(f => !f.IsStatic && (f.IsPublic || f.GetCustomAttribute<SerializeField>() != null))
                         .ToList();

        if (fields.Count > 0 && checkFieldValues)
        {
            bool allDefault = true;
            foreach (var field in fields)
            {
                var value = field.GetValue(comp);
                object defaultValue = null;
                if (field.FieldType.IsValueType)
                    defaultValue = System.Activator.CreateInstance(field.FieldType);
                if (value != null && !value.Equals(defaultValue))
                {
                    allDefault = false;
                    break;
                }
            }
            if (!allDefault) return false;
        }
        else if (fields.Count > 0) return false;

        string[] lifecycleMethods = { "Awake", "Start", "Update", "FixedUpdate", "LateUpdate",
                                      "OnEnable", "OnDisable", "OnDestroy", "OnGUI",
                                      "OnDrawGizmos", "OnDrawGizmosSelected", "Reset", "OnValidate" };
        foreach (var name in lifecycleMethods)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (method != null && method.DeclaringType == type) return false;
        }

        if (type.GetCustomAttribute<ExecuteInEditMode>() != null ||
            type.GetCustomAttribute<RequireComponent>() != null) return false;

        return true;
    }

    private static string GetPath(GameObject go)
    {
        string path = go.name;
        Transform parent = go.transform.parent;
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        return path;
    }

    private void RemoveComponent(FoundComponent item)
    {
        if (item.component != null)
        {
            Undo.DestroyObjectImmediate(item.component);
            Debug.Log($"Удалён компонент {item.componentName} с объекта {item.gameObject.name}");
        }
        foundComponents.Remove(item);
    }

    private class FoundComponent
    {
        public GameObject gameObject;
        public Component component;
        public string componentName;
        public string gameObjectPath;
    }
}