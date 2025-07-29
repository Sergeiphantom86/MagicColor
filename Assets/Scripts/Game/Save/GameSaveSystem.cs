using UnityEngine;
using YG;

public class GameSaveSystem : MonoBehaviour
{
    

    public void SaveNPCState(string npcKey, int state)
    {
        if (string.IsNullOrEmpty(npcKey))
        {
            Debug.LogError("NPC key is null or empty! Save aborted.");
            return;
        }

        if (YG2.saves == null || YG2.saves.NpcStates == null)
            return;

        if (YG2.saves.NpcStates.ContainsKey(npcKey))
            YG2.saves.NpcStates[npcKey] = state;
        else
            YG2.saves.NpcStates.Add(npcKey, state);

        YG2.SaveProgress();
    }

    public int LoadNPCState(string npcKey, int defaultState = 0)
    {
        if (YG2.saves == null || YG2.saves.NpcStates == null)
            return defaultState;

        if (YG2.saves.NpcStates.TryGetValue(npcKey, out int state))
        {
            return state;
        }

        return defaultState;
    }
}