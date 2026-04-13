using UnityEngine;

public class HintSwitcher : MonoBehaviour
{
    [SerializeField] private Blinker _backlightPause;
    [SerializeField] private Blinker _backlightPurchase;
    [SerializeField] private Blinker _backlightAbility;
    
    private HintCounter _hintCounter;

    private void Awake()
    {
        _hintCounter = GetComponent<HintCounter>();
    }

    private void OnEnable()
    {
        _backlightAbility.OnCompleted += TurnOnTimer;
        _hintCounter.OnWorked += TurnOnBacklightPause;
        _backlightPause.OnCompleted += TurnOnBacklightPurchase;
        _backlightPurchase.OnCompleted += TurnOnBacklightAbility;
    }

    private void OnDisable()
    {
        _hintCounter.OnWorked -= TurnOnBacklightPause;
        _backlightPause.OnCompleted -= TurnOnBacklightPurchase;
        _backlightPurchase.OnCompleted -= TurnOnBacklightAbility;
        _backlightAbility.OnCompleted -= TurnOnTimer;
    }

    private void TurnOnBacklightPause()
    {
        _backlightPause.Activate();
    }

    private void TurnOnBacklightPurchase()
    {
        _backlightPurchase.Activate();
    }

    private void TurnOnBacklightAbility()
    {
        _backlightAbility.Activate();
    }

    private void TurnOnTimer()
    {
        
    }
}