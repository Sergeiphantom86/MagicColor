using System.Collections;
using UnityEngine;

namespace PuzzleResources.PenEditor
{
    public interface IMover
    {
        public IEnumerator MoveToPosition(Vector3 targetPosition, float duration);
    }
}