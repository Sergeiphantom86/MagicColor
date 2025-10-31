using DG.Tweening;
using UnityEngine;

public class DOTweenInitializer : MonoBehaviour
{
    void Awake()
    {
        DOTween.Init(recycleAllByDefault: false, useSafeMode: true, logBehaviour: LogBehaviour.Verbose);
        DOTween.SetTweensCapacity(200, 50);
    }
}