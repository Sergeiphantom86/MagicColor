using UnityEngine;

public class Prompter : MonoBehaviour
{
    [SerializeField] private Pin _pin;

    public void turnOff()
    {
        _pin.TurnOff();
    }
    public void turnOn()
    {
        _pin.TurnOn();
    }
}