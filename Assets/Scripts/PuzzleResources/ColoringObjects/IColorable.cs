using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public interface IColorable
    {
        public bool IsRepainted { get; }

        public void SetOriginalColor(Color color);

        public void InstallRepainted();
    }
}