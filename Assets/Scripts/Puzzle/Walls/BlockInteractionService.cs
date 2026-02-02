using System;
using UnityEngine;

public class BlockInteractionService : IBlockInteractionService
{
    private readonly Wall _wall;
    private readonly IBlockDestroySequence _destroySequence;

    private bool _isOpen;
    private ErrorPanel _errorPanel;

    public bool IsOpen => _isOpen;

    public BlockInteractionService(Wall wall, IBlockDestroySequence destroySequence)
    {
        _wall = wall;
        _destroySequence = destroySequence;

        if (_destroySequence == null)
        {
            Debug.LogError("IBlockDestroySequence == null");
            return;
        }

        if (_wall == null)
        {
            Debug.LogError("Wall == null");
            return;
        }
    }

    public void SetPanelError(ErrorPanel errorPanel)
    {
        if(errorPanel == null)
        {
            Debug.LogError("ErrorPanel == null");
            return;
        }
        
        _errorPanel = errorPanel;
    }

    public void TryHandle(IColorable colorable, Color color)
    {
        if ( color == null)
        {
            Debug.LogError("Color == null");
            return;
        }

        if (colorable == null)
        {
            Debug.LogError("ErrorPanel == null");
            return;
        }

        if (colorable is not Block block)
            return;

        if (_wall.IsBlocked)
        {
            _errorPanel.TurnOn();
            _isOpen = true;
            return;
        }

        _destroySequence.WaitStart(block, color, _wall);
    }
}