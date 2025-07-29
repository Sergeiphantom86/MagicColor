using UnityEngine;
using YG;

public class LeaderboardManager : MonoBehaviour
{
    public void SubmitBestTime(string name, float bestTime)
    {
        if (bestTime < float.MaxValue)
        {
            YG2.SetLBTimeConvert(name, bestTime);
        }
    }
}