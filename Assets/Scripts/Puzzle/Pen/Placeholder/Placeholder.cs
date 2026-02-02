using UnityEngine;

public class Placeholder : ColorableObject
{
    private IColorable _colorable;
    private AnimatorPenFilling _animatorPenFilling;
    private EndPoint _endPoint;
    private int _size;

    public Vector3 PositionEndPoint => _endPoint.transform.position;
    public float Duration => _animatorPenFilling.GetDuration();

    private void Awake()
    {
        _colorable = GetComponent<IColorable>();
        _animatorPenFilling = GetComponent<AnimatorPenFilling>();
        _endPoint = GetComponentInChildren<EndPoint>();

        InitializeComponents();
    }

    public void ShowFillings(Color color, float nextSpawnYPosition)
    {
        SetPosition(nextSpawnYPosition);
        SetActive(true);

        IncreaseSize(color);
        Repaint(color);
        SetSize();
    }

    public void ReduceSize()
    {
        _size -= 1;
        
        if (_size == 0)
        {
            Destroy(this);
        }
    }

    private void SetSize()
    {
        _size = _animatorPenFilling.Size;
    }

    private void IncreaseSize(Color color)
    {
        _animatorPenFilling.FillPen(color, this);
    }

    private void Repaint(Color color)
    {
        _colorable.SetColor(color);
        _colorable.SetAlpha(1);
        EnableEmission(color);
    }

    private void SetPosition(float nextSpawnYPosition)
    {
        transform.localPosition = new Vector3(0, nextSpawnYPosition, 0);
    }
}