using Menu.TutorialEditor.TutorialPuzzle;
using PuzzleEditor.LockEditor;

namespace PuzzleEditor.Walls
{
    public interface ILockFeedbackService
    {
        public void InitializComponents(Lock @lock, Messager hint);

        void Play();
    }
}