using System;
using UnityEngine;

namespace Menu.Tutorials
{
    public class CollisionHandler : MonoBehaviour, ICollisionHandler
    {
        public event Action<Collider> Enter;

        public event Action<Collider> Exit;

        private void OnTriggerEnter(Collider other)
        {
            Enter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            Exit?.Invoke(other);
        }
    }
}