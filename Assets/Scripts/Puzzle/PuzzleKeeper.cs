using System.Collections.Generic;
using UnityEngine;

public class PuzzleKeeper : MonoBehaviour
{
    public Dictionary<string, float> BestTimes;

    private void Awake()
    {
        BestTimes = new Dictionary<string, float>();
    }
}