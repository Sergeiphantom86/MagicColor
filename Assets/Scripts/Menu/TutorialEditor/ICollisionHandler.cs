using System;
using UnityEngine;
namespace Menu.TutorialEditor
{

public interface ICollisionHandler
{
    public event Action<Collider> OnEnter;

    public event Action<Collider> OnExit;
}
}