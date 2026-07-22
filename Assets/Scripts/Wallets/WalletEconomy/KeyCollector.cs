using Menu.Tutorials;
using UnityEngine;

namespace Wallets.WalletEconomy
{
    public class KeyCollector : MonoBehaviour
    {
        [SerializeField] private BagKey _bagKey;

        private ICollisionHandler _collisionHandler;

        private void Awake()
        {
            _collisionHandler = GetComponent<ICollisionHandler>();
        }

        private void OnEnable()
        {
            _collisionHandler.Entered += OnAdd;
        }

        private void OnDisable()
        {
            _collisionHandler.Entered -= OnAdd;
        }

        private void OnAdd(Collider collider)
        {
            if (collider.TryGetComponent(out Key key))
            {
                if (key == null)
                return;

                _bagKey.Add();
                key.gameObject.SetActive(false);
            }
        }
    }
}