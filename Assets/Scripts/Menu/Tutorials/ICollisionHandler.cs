using System;
using UnityEngine;

namespace Menu.Tutorials
{
    public interface ICollisionHandler
    {
        public event Action<Collider> Enter;

        public event Action<Collider> Exit;
    }
}