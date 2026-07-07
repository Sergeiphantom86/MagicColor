using PuzzleEditor;
using PuzzleEditor.Counter;
using PuzzleEditor.LockMechanics;
using PuzzleEditor.Spawners;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle.TutorialStateMachine
{
    public class TutorialContext
    {
        private readonly float _delay = 0.5f;
        private readonly float _starShutdownDelay = 4;
        private readonly float _delayDisablingUI = 7;

        public TutorialContext()
        {
            WaitForSeconds = new WaitForSeconds(_delay);
            WaitFirstStop = new WaitForSeconds(_delay / _delay * 2);
            WaitStarTurnOff = new WaitForSeconds(_starShutdownDelay);
            WaitUIDisabled = new WaitForSeconds(_delayDisablingUI);
        }

        public WaitForSeconds WaitFirstStop { get; }
        public WaitForSeconds WaitForSeconds { get; }
        public WaitForSeconds WaitUIDisabled { get; }
        public WaitForSeconds WaitStarTurnOff { get; }
        public Key Key { get; private set; }
        public Lock Lock { get; private set; }
        public Hints Hints { get; private set; }
        public Timer Timer { get; private set; }
        public Rotator Rotator { get; private set; }
        public HandMover HandMover { get; private set; }
        public BlockSpawner Container { get; private set; }
        public TouchVisualizer Visualizer { get; private set; }
        public StateTutorial StateTutorial { get; private set; }
        public TutorialAbilities TutorialAbilities { get; private set; }
        public void InitBase(HandMover handMover, TouchVisualizer visualizer)
        {
            HandMover = handMover;
            Visualizer = visualizer;
        }

        public void InitScene(Key key,
        Lock @lock,
        Hints hints,
        Timer timer,
        Rotator rotator,
        BlockSpawner container,
        StateTutorial stateTutorial,
        TutorialAbilities tutorialAbilities)
        {
            Key = key;
            Lock = @lock;
            Hints = hints;
            Timer = timer;
            Rotator = rotator;
            Container = container;
            StateTutorial = stateTutorial;
            TutorialAbilities = tutorialAbilities;
        }

        public void AdjustPositions(
            Vector3? handPosition = null,
            Vector3? visualizerPosition = null,
            float yOffset = 0f)
        {
            SetObjectPosition(GetTransform(HandMover), handPosition, 0, yOffset, 0);
            SetObjectPosition(GetTransform(Visualizer), visualizerPosition, 0, yOffset, 0);
        }

        private Transform GetTransform(Component component)
        {
            return component != null ? component.transform : null;
        }

        private void SetObjectPosition(
            Transform targetTransform,
            Vector3? position,
            float xOffset,
            float yOffset,
            float zOffset)
        {
            if (targetTransform != null && position.HasValue)
            {
                targetTransform.position = GetPosition(position.Value, xOffset, yOffset, zOffset);
            }
        }

        private Vector3 GetPosition(Vector3 position, float xOffset, float yOffset, float zOffset)
        {
            position.x += xOffset;
            position.y += yOffset;
            position.z += zOffset;
            return position;
        }
    }
}