using System;
using UnityEngine;

namespace Menu.TutorialEditor
{
    public interface ICollisionHandler
    {
        public event Action<Collider> Enter;

        public event Action<Collider> Exit;
    }
}