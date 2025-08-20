using UnityEngine;

public class GameSaveSystem : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Current saved value (non-negative)")]
    private int _currentValue;

    public int CurrentValue
    {
        get => _currentValue;
        set
        {
            if (value < 0)
            {
                Debug.LogWarning($"Attempted to set negative value: {value}. Using 0 instead.");
                _currentValue = 0;
            }
            else
            {
                _currentValue = value;
            }
        }
    }
}