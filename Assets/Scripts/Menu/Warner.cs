using UnityEngine;

public class Warner : MonoBehaviour
{
    private void Awake()
    {
        TurnOff();
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }

    public void TurnOn()
    {
        gameObject.SetActive(true);
    }
}