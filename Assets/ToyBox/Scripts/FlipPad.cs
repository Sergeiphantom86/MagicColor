using UnityEngine;
using PuzzleResources.UI;

public class FlipPad : MonoBehaviour
{
    private AnimatorPuzzle ani;

    public bool state = false;

    public float flipTime = 4;

    void Start()
    {
        ani = gameObject.GetComponent<AnimatorPuzzle>();
        state = false;
    }
}