using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float _hoverScale;
    [SerializeField] private float _hoverAlpha;
    [SerializeField] private float _animationDuration;

    private Vector3 _originalScale;
    private CanvasGroup _canvasGroup;
    private float _originalAlpha;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _originalScale = transform.localScale;
        _originalAlpha = _canvasGroup.alpha;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(_originalScale * _hoverScale, _animationDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(_originalScale, _animationDuration);
    }

    private void OnDisable()
    {
        transform.localScale = _originalScale;
        if (_canvasGroup) _canvasGroup.alpha = _originalAlpha;
    }
}