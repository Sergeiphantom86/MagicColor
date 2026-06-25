using System.Collections;
using UnityEngine;
namespace PuzzleEditor.PenEditor
{

public interface IMover
{
    public IEnumerator MoveToPosition(Vector3 targetPosition, float duration);
}
}