using UnityEngine;

public class TexturePuzzleRevealer : MonoBehaviour
{
    [Header("Source texture")]
    [SerializeField] private Texture2D _sourceTexture;

    [Header("Slices (from Sprite Editor)")]
    [SerializeField] private Sprite[] _slices;

    [Header("Renderer")]
    [SerializeField] private SpriteRenderer _renderer;

    private Texture2D _runtimeTexture;
    private bool[] _opened;
    private int _openedCount;
    private bool _dirty;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (_dirty)
        {
            _runtimeTexture.Apply();
            _dirty = false;
        }
    }

    private void Init()
    {
        if (_renderer == null)
            _renderer = GetComponent<SpriteRenderer>();

        _runtimeTexture = new Texture2D(
            _sourceTexture.width,
            _sourceTexture.height,
            TextureFormat.RGBA32,
            false
        );

        _runtimeTexture.filterMode = FilterMode.Point;
        _runtimeTexture.wrapMode = TextureWrapMode.Clamp;

        ClearTexture();

        _renderer.sprite = Sprite.Create(
            _runtimeTexture,
            new Rect(0, 0, _runtimeTexture.width, _runtimeTexture.height),
            new Vector2(0.5f, 0.5f),
            1
        );

        _opened = new bool[_slices.Length];
        _openedCount = 0;
    }

    private void ClearTexture()
    {
        Color[] clear = new Color[_runtimeTexture.width * _runtimeTexture.height];
        for (int i = 0; i < clear.Length; i++)
            clear[i] = Color.clear;

        _runtimeTexture.SetPixels(clear);
        _runtimeTexture.Apply();
    }

    /// <summary>
    /// Открывает участок по индексу спрайта
    /// </summary>
    public void RevealSlice(int sliceIndex)
    {
        if (sliceIndex < 0 || sliceIndex >= _slices.Length)
            return;

        if (_opened[sliceIndex])
            return;

        _opened[sliceIndex] = true;
        _openedCount++;

        DrawSlice(_slices[sliceIndex]);

        if (_openedCount >= _slices.Length)
            RevealAll();
    }

    private void DrawSlice(Sprite slice)
    {
        Rect rect = slice.rect;

        int x = Mathf.FloorToInt(rect.x);
        int y = Mathf.FloorToInt(rect.y);
        int w = Mathf.FloorToInt(rect.width);
        int h = Mathf.FloorToInt(rect.height);

        Color[] pixels = _sourceTexture.GetPixels(x, y, w, h);
        _runtimeTexture.SetPixels(x, y, w, h, pixels);

        _dirty = true;
    }

    private void RevealAll()
    {
        // мгновенно показываем всю картинку
        _runtimeTexture.SetPixels(_sourceTexture.GetPixels());
        _runtimeTexture.Apply();
    }
}
