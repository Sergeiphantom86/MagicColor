using UnityEngine;

namespace PuzzleEditor
{
    public interface IColorable
    {
        public bool IsRepainted { get; }

        public void SetColor(Color color);

        public Color GetColor();

        public void InstallRepainted();

        public void AssignOriginal();

        public void Disable();

        public void SetAlpha(float alpha);

        public void SetRenderQueue();

        public void SetStartRenderQueueSelectedItem();

        public void SetRenderQueueSelectedItem();
    }
}