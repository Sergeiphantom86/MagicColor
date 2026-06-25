using UnityEngine;

namespace PuzzleEditor.Table
{
    public class ShaderUpdaterr : MonoBehaviour
    {
        [SerializeField]
        private float _multiplier;

        private GridPlaneFiller _gridPlaneFiller;

        private void Awake()
        {
            _gridPlaneFiller = GetComponent<GridPlaneFiller>();
        }

        private void OnEnable()
        {
            _gridPlaneFiller.HasChanged += UpdateGridAfterScale;
        }

        private void OnDisable()
        {
            _gridPlaneFiller.HasChanged -= UpdateGridAfterScale;
        }

        private void UpdateGridAfterScale(
            Material material,
            int gridSizeX,
            int gridSizeY,
            float cellSize
        )
        {
            material.SetVector(
                "_GridSize",
                new Vector4(
                    cellSize * gridSizeX / _multiplier,
                    cellSize * gridSizeY / _multiplier,
                    0,
                    0
                )
            );
        }
    }
}