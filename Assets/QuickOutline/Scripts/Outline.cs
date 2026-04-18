using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Outline : MonoBehaviour
{
    private const int UV_CHANNEL_SMOOTH_NORMALS = 3;
    private const float MAX_OUTLINE_WIDTH = 10f;
    private const float OUTLINE_WIDTH_ZERO = 0f;
    private const int SINGLE_SUBMESH = 1;
    private const int VERTEX_GROUP_SINGLE = 1;

    private static HashSet<Mesh> _registeredMeshes = new HashSet<Mesh>();

    public enum Mode
    {
        OutlineAll,
        OutlineVisible,
        OutlineHidden,
        OutlineAndSilhouette,
        SilhouetteOnly
    }

    public Mode OutlineMode
    {
        get { return _outlineMode; }
        set
        {
            _outlineMode = value;
            _needsUpdate = true;
        }
    }

    public Color OutlineColor
    {
        get { return _outlineColor; }
        set
        {
            _outlineColor = value;
            _needsUpdate = true;
        }
    }

    public float OutlineWidth
    {
        get { return _outlineWidth; }
        set
        {
            _outlineWidth = value;
            _needsUpdate = true;
        }
    }

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> _data;
    }

    [SerializeField]
    private Mode _outlineMode;

    [SerializeField]
    private Color _outlineColor = Color.white;

    [SerializeField, Range(0f, MAX_OUTLINE_WIDTH)]
    private float _outlineWidth = 2f;

    [Header("Optional")]
    [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
    + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
    private bool _precomputeOutline;

    [SerializeField, HideInInspector]
    private List<Mesh> _bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> _bakeValues = new List<ListVector3>();

    private Renderer[] _renderers;
    private Material _outlineMaskMaterial;
    private Material _outlineFillMaterial;

    private bool _needsUpdate;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();

        _outlineMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineMask"));
        _outlineFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/OutlineFill"));

        _outlineMaskMaterial.name = "OutlineMask (Instance)";
        _outlineFillMaterial.name = "OutlineFill (Instance)";

        LoadSmoothNormals();

        _needsUpdate = true;
    }

    private void OnEnable()
    {
        foreach (var renderer in _renderers)
        {
            var materials = renderer.sharedMaterials.ToList();

            materials.Add(_outlineMaskMaterial);
            materials.Add(_outlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    private void OnValidate()
    {
        _needsUpdate = true;

        if (_precomputeOutline == false && _bakeKeys.Count != 0 || _bakeKeys.Count != _bakeValues.Count)
        {
            _bakeKeys.Clear();
            _bakeValues.Clear();
        }

        if (_precomputeOutline && _bakeKeys.Count == 0)
        {
            Bake();
        }
    }

    private void Update()
    {
        if (_needsUpdate)
        {
            _needsUpdate = false;
            UpdateMaterialProperties();
        }
    }

    private void OnDisable()
    {
        foreach (var renderer in _renderers)
        {
            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(_outlineMaskMaterial);
            materials.Remove(_outlineFillMaterial);

            renderer.materials = materials.ToArray();
        }
    }

    private void OnDestroy()
    {
        Destroy(_outlineMaskMaterial);
        Destroy(_outlineFillMaterial);
    }

    private void Bake()
    {
        var bakedMeshes = new HashSet<Mesh>();

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (bakedMeshes.Add(meshFilter.sharedMesh) == false)
            {
                continue;
            }

            var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

            _bakeKeys.Add(meshFilter.sharedMesh);
            _bakeValues.Add(new ListVector3() { _data = smoothNormals });
        }
    }

    private void LoadSmoothNormals()
    {
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (_registeredMeshes.Add(meshFilter.sharedMesh) == false)
            {
                continue;
            }

            var index = _bakeKeys.IndexOf(meshFilter.sharedMesh);
            var smoothNormals = (index >= 0) ? _bakeValues[index]._data : SmoothNormals(meshFilter.sharedMesh);

            meshFilter.sharedMesh.SetUVs(UV_CHANNEL_SMOOTH_NORMALS, smoothNormals);

            var renderer = meshFilter.GetComponent<Renderer>();

            if (renderer != null)
            {
                CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
            }
        }

        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (_registeredMeshes.Add(skinnedMeshRenderer.sharedMesh) == false)
            {
                continue;
            }

            skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

            CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
        }
    }

    private List<Vector3> SmoothNormals(Mesh mesh)
    {
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        var smoothNormals = new List<Vector3>(mesh.normals);

        foreach (var group in groups)
        {
            if (group.Count() == VERTEX_GROUP_SINGLE)
            {
                continue;
            }

            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += smoothNormals[pair.Value];
            }

            smoothNormal.Normalize();

            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    private void CombineSubmeshes(Mesh mesh, Material[] materials)
    {
        if (mesh.subMeshCount == SINGLE_SUBMESH)
        {
            return;
        }

        if (mesh.subMeshCount > materials.Length)
        {
            return;
        }

        mesh.subMeshCount++;
        mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
    }

    private void UpdateMaterialProperties()
    {
        _outlineFillMaterial.SetColor("_OutlineColor", _outlineColor);

        switch (_outlineMode)
        {
            case Mode.OutlineAll:
                SetOutlineProperties(UnityEngine.Rendering.CompareFunction.Always, UnityEngine.Rendering.CompareFunction.Always, _outlineWidth);
                break;

            case Mode.OutlineVisible:
                SetOutlineProperties(UnityEngine.Rendering.CompareFunction.Always, UnityEngine.Rendering.CompareFunction.LessEqual, _outlineWidth);
                break;

            case Mode.OutlineHidden:
                SetOutlineProperties(UnityEngine.Rendering.CompareFunction.Always, UnityEngine.Rendering.CompareFunction.Greater, _outlineWidth);
                break;

            case Mode.OutlineAndSilhouette:
                SetOutlineProperties(UnityEngine.Rendering.CompareFunction.LessEqual, UnityEngine.Rendering.CompareFunction.Always, _outlineWidth);
                break;

            case Mode.SilhouetteOnly:
                SetOutlineProperties(UnityEngine.Rendering.CompareFunction.LessEqual, UnityEngine.Rendering.CompareFunction.Greater, OUTLINE_WIDTH_ZERO);
                break;
        }
    }

    private void SetOutlineProperties(UnityEngine.Rendering.CompareFunction maskZTest, UnityEngine.Rendering.CompareFunction fillZTest, float outlineWidth)
    {
        _outlineMaskMaterial.SetFloat("_ZTest", (float)maskZTest);
        _outlineFillMaterial.SetFloat("_ZTest", (float)fillZTest);
        _outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
    }
}