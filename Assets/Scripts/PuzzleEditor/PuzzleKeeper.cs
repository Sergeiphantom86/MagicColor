using System.Collections.Generic;
using UnityEngine;

namespace PuzzleEditor
{
    public class PuzzleKeeper : MonoBehaviour
    {
        public Dictionary<string, float> BestTimes;

        private void Awake()
        {
            BestTimes = new Dictionary<string, float>();
        }
    }
}