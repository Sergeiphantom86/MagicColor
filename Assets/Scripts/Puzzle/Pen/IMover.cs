using System.Collections;
using UnityEngine;

public interface IMover
{
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration);
}