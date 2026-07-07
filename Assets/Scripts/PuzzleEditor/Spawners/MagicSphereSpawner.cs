using Menu.Interaction.Ability;
using PuzzleEditor.EnergyField;
using PuzzleEditor.MovingBlocks;
using PuzzleEditor.MovingBlocks.GridLogic;
using PuzzleEditor.Audio;
using UnityEngine;

namespace PuzzleEditor.Spawners
{
    public class MagicSphereSpawner : BaseSpawner<MagicSphere>
    {
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private AbilitySelectionManager _abilitySelectionManager;

        private GridSystem _grid;
        private IInputHandler _input;
        private Voiceover _voiceover;

        protected override void Awake()
        {
            base.Awake();

            _input = GetComponent<IInputHandler>();
            _voiceover = GetComponent<Voiceover>();
            _grid = GetComponent<GridSystem>();

            if (_input == null)
                Debug.LogError("InputHandler == null");
        }

        private void Start()
        {
            //if (_grid == null)
            //    _grid = GridSystem.Instance;
        }

        private void OnEnable()
        {
            if (_input != null)
            {
                _input.Selected += OnTrySpawnAtWorldPos;
            }

            DespawnAll();
        }

        private void OnDisable()
        {
            if (_input != null)
            {
                _input.Selected -= OnTrySpawnAtWorldPos;
            }
        }

        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3Int cell = _grid.GetComponent<Grid>().WorldToCell(worldPos);
            return new Vector2Int(cell.x, cell.y);
        }

        public void DespawnAll()
        {
            for (int i = SpawnedObjects.Count - 1; i >= 0; i--)
            {
                Despawn(SpawnedObjects[i]);
            }
        }

        private void OnTrySpawnAtWorldPos(Vector2 screenPos)
        {
            if (_abilitySelectionManager.HasSelection == false)
                return;

            Vector2Int origin = _grid.GetOriginFromCenter(WorldToGrid(_input.Point), Vector2Int.one);

            if (_input.Point == Vector3.zero)
                return;

            _abilitySelectionManager.Use();
            _voiceover.PlayOneShot(_audioClip);

            Vector3 spawnPos = _grid.GetWorldPosition(origin, Vector2Int.one);

            MagicSphere sphere = SpawnObjectWithCurrentIndex(spawnPos, transform);

            sphere.transform.position = spawnPos;

            _grid.PlaceObject(origin, sphere);

            _abilitySelectionManager.ClearSelection();
        }
    }
}