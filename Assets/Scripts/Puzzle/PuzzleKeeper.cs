using System.Collections.Generic;
using UnityEngine;

public class PuzzleKeeper : MonoBehaviour
{
    public Dictionary<string, float> _bestTimes;

    private void Awake()
    {
        _bestTimes = new Dictionary<string, float>();
    }
}
