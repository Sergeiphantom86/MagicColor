using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HandMover))]
public class HandIndicator : MonoBehaviour
{
    [SerializeField] Button _buttonBack;

    private HandMover _handMover;

    private void Awake()
    {
        _handMover = GetComponent<HandMover>();
    }

    private void OnEnable()
    {
        _buttonBack.onClick.AddListener(TurnOff);
    }

    private void TurnOff()
    {
        _handMover.OnDestroyed();
    }
}