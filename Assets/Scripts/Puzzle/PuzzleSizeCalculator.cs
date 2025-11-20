using UnityEngine;

public class PuzzleSizeCalculator
{
    private readonly Camera _targetCamera;
    private readonly float _screenCoverage;
    private readonly float _maxPuzzleSize;

    public PuzzleSizeCalculator(Camera targetCamera, float screenCoverage, float maxPuzzleSize)
    {
        _targetCamera = targetCamera;
        _screenCoverage = screenCoverage;
        _maxPuzzleSize = maxPuzzleSize;
    }

    public PuzzleSizeData CalculatePuzzleSize(float textureWidth, float textureHeight)
    {
        var (Width, Height) = GetScreenWorldSize();
        float targetSize = CalculateTargetSize(Height);
        var puzzleDimensions = CalculatePuzzleDimensions(targetSize, textureWidth, textureHeight);
        float pixelSize = CalculatePixelSize(puzzleDimensions.Width, puzzleDimensions.Height, textureWidth, textureHeight);

        return new PuzzleSizeData
        {
            PixelSize = pixelSize,
            PuzzleWorldWidth = puzzleDimensions.Width,
            PuzzleWorldHeight = puzzleDimensions.Height,
            ScreenWorldWidth = Width,
            ScreenWorldHeight = Height
        };
    }

    private (float Width, float Height) GetScreenWorldSize()
    {
        float screenWorldHeight = _targetCamera.orthographicSize;
        float screenWorldWidth = _targetCamera.orthographicSize * _targetCamera.aspect;
        return (screenWorldWidth, screenWorldHeight);
    }

    private float CalculateTargetSize(float screenWorldHeight)
    {
        float targetSize = screenWorldHeight * _screenCoverage;
        return Mathf.Min(targetSize, _maxPuzzleSize);
    }

    private (float Width, float Height) CalculatePuzzleDimensions(float targetSize, float textureWidth, float textureHeight)
    {
        float textureAspect = textureWidth / textureHeight;
        float puzzleWidth = targetSize;
        float puzzleHeight = targetSize / textureAspect;

        if (puzzleHeight > _targetCamera.orthographicSize * _screenCoverage)
        {
            puzzleHeight = _targetCamera.orthographicSize * _screenCoverage;
            puzzleWidth = puzzleHeight * textureAspect;
        }

        return (puzzleWidth, puzzleHeight);
    }

    private float CalculatePixelSize(float puzzleWidth, float puzzleHeight, float textureWidth, float textureHeight)
    {
        return Mathf.Min(puzzleWidth / textureWidth, puzzleHeight / textureHeight);
    }
}

public struct PuzzleSizeData
{
    public float PixelSize { get; set; }
    public float PuzzleWorldWidth { get; set; }
    public float PuzzleWorldHeight { get; set; }
    public float ScreenWorldWidth { get; set; }
    public float ScreenWorldHeight { get; set; }
}