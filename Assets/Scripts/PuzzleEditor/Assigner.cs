using Menu.TutorialEditor;
using PuzzleEditor.LockEditor;
using UnityEngine;

namespace PuzzleEditor
{
    public class Assigner : MonoBehaviour
    {
        private ICollisionHandler _collisionHandler;

        private void Awake()
        {
            _collisionHandler = GetComponent<ICollisionHandler>();
        }

        private void OnEnable()
        {
            _collisionHandler.OnEnter += SetParent;
        }

        private void OnDisable()
        {
            _collisionHandler.OnEnter -= SetParent;
        }

        private void SetParent(Collider collider)
        {
            if (collider.TryGetComponent(out Lock @lock))
            {
                Debug.Log(10);

                @lock.transform.SetParent(transform);
            }
        }
    }
}