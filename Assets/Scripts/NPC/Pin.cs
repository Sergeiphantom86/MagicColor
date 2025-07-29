using UnityEngine;

public class Pin : MonoBehaviour
{
    private Pin GetPrefab()
    {
        return this;
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