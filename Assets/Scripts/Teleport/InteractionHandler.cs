using System;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public event Action<Player> OnContact;
    public event Action<string> OnContactTerrain;
    public event Action ContactsFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
            OnContact?.Invoke(player);

        if (other.TryGetComponent(out TeleportationPoint teleportationPoint)) 
            OnContactTerrain?.Invoke(teleportationPoint.name);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Player player)) 
            ContactsFinished?.Invoke();
    }
}