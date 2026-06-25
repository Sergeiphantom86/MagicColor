using UnityEngine;
namespace PuzzleEditor.UI
{

public class VictoryPlaque : MonoBehaviour
{
    private void Awake()
    {
        TurnOff();
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
}