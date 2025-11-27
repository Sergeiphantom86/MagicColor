using UnityEngine;

public class TutorialContext
{
    private readonly float _delay = 1f;

    public Key Key { get; set; }
    public Lock Lock { get; set; }
    public Hints Hints { get; set; }
    public Mirage Mirage { get; set; }
    public Rotator Rotator { get; set; }
    public Block CurrentBlock { get; set; }
    public HandMover HandMover { get; set; }
    public MenuLoader MenuLoader { get; set; }
    public BlockSpawner Container { get; set; }
    public bool IsAnimationChange { get; set; }
    public WaitForSeconds WaitForSeconds { get; }
    public TouchVisualizer Visualizer { get; set; }
    public StateTutorial StateTutorial { get; set; }
    public TouchDragInput CurrentTouchInput { get; set; }

    public TutorialContext()
    {
        WaitForSeconds = new WaitForSeconds(_delay);
    }

    public void AdjustPositions(Vector3? handPosition = null, Vector3? visualizerPosition = null, Vector3? miragePosition = null, float yOffset = 0f)
    {
        SetObjectPosition(GetTransform(HandMover), handPosition, 0, yOffset - 0.2f, 0);
        SetObjectPosition(GetTransform(Visualizer), visualizerPosition, 0, yOffset, 0);
        SetObjectPosition(GetTransform(Mirage), miragePosition, 0, -yOffset - 0.2f, 0);
    }

    private Transform GetTransform(Component component)
    {
        return component != null ? component.transform : null;
    }

    private void SetObjectPosition(Transform targetTransform, Vector3? position, float xOffset, float yOffset, float zOffset)
    {
        if (targetTransform != null && position.HasValue)
        {
            targetTransform.position = CalculatePosition(position.Value, xOffset, yOffset, zOffset);
        }
    }

    private Vector3 CalculatePosition(Vector3 position, float xOffset, float yOffset, float zOffset)
    {
        position.x += xOffset;
        position.y += yOffset;
        position.z += zOffset;
        return position;
    }
}