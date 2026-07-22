using UnityEngine;

public static class ValidationHelper
{
    public static bool AllNotNull(MonoBehaviour context, params (object dependency, string name)[] dependencies)
    {
        foreach (var (dependency, name) in dependencies)
        {
            if (dependency == null)
            {
                Debug.LogError($"[{context.GetType().Name}] Initialization failed: {name} is NULL", context);
                return false;
            }
        }
        return true;
    }
}