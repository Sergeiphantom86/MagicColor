using UnityEngine;

public class ColorSubstitute : ColorableObject
{
    [Header("Настройки цвета")]
    [SerializeField] private bool changeOnAwake = true;
    [SerializeField] private bool useSaturationRange = false;
    [SerializeField][Range(0f, 1f)] private float minSaturation = 0.5f;
    [SerializeField][Range(0f, 1f)] private float maxSaturation = 1f;
    [SerializeField] private bool useValueRange = false;
    [SerializeField][Range(0f, 1f)] private float minValue = 0.5f;
    [SerializeField][Range(0f, 1f)] private float maxValue = 1f;

    private void Awake()
    {
        InitializeComponents();

        if (changeOnAwake)
        {
            ChangeToRandomColor();
        }
    }

    public void ChangeToRandomColor()
    {
        Color randomColor;

        if (useSaturationRange || useValueRange)
        {
            randomColor = Random.ColorHSV(
                0f, 1f,
                useSaturationRange ? minSaturation : 0f,
                useSaturationRange ? maxSaturation : 1f,
                useValueRange ? minValue : 0f,
                useValueRange ? maxValue : 1f
            );
        }
        else
        {
            randomColor = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );
        }

        SetColor(randomColor);
        InstallRepainted();
        AssignOriginal();
    }
}