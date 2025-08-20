using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private List<IAnimatable> _animatables;

    private void Awake()
    {
        _animatables = new List<IAnimatable>();

        GetComponents(_animatables);
    }

    public void PauseAllAnimations()
    {
        if (_animatables != null)
        {
            foreach (var animatable in _animatables)
            {
                animatable.PauseAnimations();
            }
        }
    }

    public void ResumeAllAnimations()
    {
        if (_animatables != null)
        {
            foreach (var animatable in _animatables)
            {
                animatable.ResumeAnimations();
            }
        }
    }
}