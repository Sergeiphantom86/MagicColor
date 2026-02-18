using System;
using UnityEngine;

public abstract class ResourceStorage<T> : MonoBehaviour
{
    [SerializeField] protected T _value;

    public T Value => _value;

    public abstract string Id { get; }

    public event Action<T, string> OnValueChanged;

    protected void SetValue(T value)
    {
        _value = value;
        OnValueChanged?.Invoke(_value, Id);
    }
}