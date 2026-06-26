using Menu.TutorialEditor;
using UnityEngine;

namespace Wallets.WalletEditor
{
    public class KeyCollector : MonoBehaviour
    {
        [SerializeField]
        private BagKey bagKey;

        private ICollisionHandler _collisionHandler;

        private void Awake()
        {
            _collisionHandler = GetComponent<ICollisionHandler>();
        }

        private void OnEnable()
        {
            _collisionHandler.Enter += OnAdd;
        }

        private void OnDisable()
        {
            _collisionHandler.Enter -= OnAdd;
        }

        private void OnAdd(Collider collider)
        {
            if (collider.TryGetComponent(out Key key))
            {
                if (key == null)
                    return;

                bagKey.Add();
                key.gameObject.SetActive(false);
            }
        }
    }
}