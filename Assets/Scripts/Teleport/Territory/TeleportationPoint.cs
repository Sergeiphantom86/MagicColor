using UnityEngine;

public class TeleportationPoint : MonoBehaviour
{
    private string _territoryName;

    public string TerritoryName => _territoryName;

    private void Awake()
    {
        _territoryName = gameObject.name;
    }
}