using UnityEngine;

namespace PuzzleEditor.Table
{
    public class IsoGridController : MonoBehaviour
    {
        private Renderer _renderer;
        private Material _material;
        private float _cellSize;

        private void Awake()
        {
            _cellSize = 64f;
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
        }

        private void Start()
        {
            _material.SetVector("_Resolution", new Vector4(Screen.width, Screen.height, 0, 0));
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 cell = ScreenToCell(Input.mousePosition);
                _material.SetVector("_ActiveCell", new Vector4(cell.x, cell.y, 0, 0));
            }
        }

        private Vector2 ScreenToCell(Vector2 mouse)
        {
            Vector2 uv = new(mouse.x / Screen.width, mouse.y / Screen.height);

            Vector2 world = uv * new Vector2(Screen.width, Screen.height) / _cellSize;

            float x = (world.x + world.y * 2f) * 0.5f;
            float y = (world.y * 2f - world.x) * 0.5f;

            return new Vector2(Mathf.Floor(x), Mathf.Floor(y));
        }
    }
}