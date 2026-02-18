using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ButtonCarouselController), typeof(ButtonSoundHandler))]
public class UnifiedSwipeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Swipe Settings")]
    [SerializeField] private float _swipeThreshold = 50f;
    [SerializeField] private Tutorial _tutorial;
    [SerializeField] private AudioClip _swipeSound;

    private ICarousel _carousel;
    private ButtonSoundHandler _buttonSound;
    private Vector2 _startDragPosition;
    private bool _isDragging;
    private bool _blockInputWhenTutorialActive;

    private void Awake()
    {
        _carousel = GetComponent<ICarousel>();
        _buttonSound = GetComponent<ButtonSoundHandler>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (ShouldBlockInput()) return;
        _buttonSound.PlayButtonSound(_swipeSound);
        _isDragging = true;
        _startDragPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging == false || ShouldBlockInput()) return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isDragging == false || ShouldBlockInput()) return;
        _isDragging = false;

        Vector2 direction = eventData.position - _startDragPosition;

        if (Mathf.Abs(direction.x) < _swipeThreshold) return;

        _carousel.ShowRelative((int)Mathf.Sign(-direction.x));

        if (_tutorial != null && _tutorial.IsTutorialActive)
        {
            _tutorial.CompleteSwapStep();
        }

        Vector2 delta = eventData.position - _startDragPosition;

        if (Mathf.Abs(delta.x) < 80f) return;
    }

    private bool ShouldBlockInput()
    {
        if (_blockInputWhenTutorialActive == false) return false;
        if (_tutorial == null) return false;

        return _tutorial.IsTutorialActive &&(_tutorial.IsSwipeAllowed && _isDragging) == false &&_tutorial.IsClickAllowed == false;
    }
}