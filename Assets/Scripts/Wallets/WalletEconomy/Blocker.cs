using PuzzleResources;
using UnityEngine;

namespace Wallets.WalletEconomy
{
    public class Blocker : MonoBehaviour, IActivatable
    {
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}