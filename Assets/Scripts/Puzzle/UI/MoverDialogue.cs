using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MoverDialogue : MonoBehaviour
{
    [SerializeField] private Dialogues _dialogPanel;
    [SerializeField] private Camera _camera;

    private Sequence _sequence;
    private MoverUI _moverUI;
    private float _duration;
    private Canvas _canvas;


    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        _moverUI = new MoverUI();
        _duration = 0.2f;
    }

    public void Show()
    {
        _moverUI.EnableMotionAnimation(_dialogPanel.transform, _duration, _camera, _sequence, 0.4f, 0.6f, _canvas.planeDistance);
        _moverUI.EnableAnimationResizing(_dialogPanel.transform, _duration, _sequence, 10);
    }


    public void Close()
    {
        _moverUI.EnableMotionAnimation(_dialogPanel.transform, _duration, _camera, _sequence, 1.4f, 0.5f, _canvas.planeDistance);
        _moverUI.EnableAnimationResizing(_dialogPanel.transform, _duration, _sequence, 0.1f);
    }
}
