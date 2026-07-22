using System;
using UnityEngine;

namespace Menu.Tutorials
{
    public interface ICollisionHandler
    {
        public event Action<Collider> Entered;

        public event Action<Collider> Exited;
    }
}