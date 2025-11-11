using UnityEngine;

public class Bag : Wallet
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Key key))
        {
            AddFunds(key.Value, 0.01f);
        }
    }
}