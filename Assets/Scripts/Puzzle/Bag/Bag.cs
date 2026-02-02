using UnityEngine;

public class Bag : Wallet
{
    [SerializeField] KeyCollector _keyCollector;
    [SerializeField] AnimatorPuzzle _animatorPuzzle;

    private void OnEnable()
    {
        _keyCollector.OnAdded += AddKey;
        _animatorPuzzle.OnAnimationComplete += TurnOff;
    }

    private void OnDisable()
    {
        _keyCollector.OnAdded -= AddKey;
        _animatorPuzzle.OnAnimationComplete -= TurnOff;
    }

    private void AddKey(Key key)
    {
        AddFunds(key.Value, 0.01f);
        key.gameObject.SetActive(false);
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}