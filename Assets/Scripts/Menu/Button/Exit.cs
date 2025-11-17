using UnityEngine;
using YG;

public class Exit : MonoBehaviour
{
    private int _indexCorrector;

    private void Awake()
    {
        _indexCorrector = 1;
    }

    public int GetIndex()
    {
        int index = YG2.saves.QuestIndex - _indexCorrector;
        
        return index;
    }
}