using UnityEngine;

public class ResultsKeeper : MonoBehaviour
{
    private Rewards _rewards;

    private void Awake()
    {
        _rewards = GetComponent<Rewards>();
    }

    private void OnDisable()
    {
        _rewards.Save();
    }
}