using System.Collections.Generic;
using UnityEngine;

public class StablePhysicsRope : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private int segmentCount = 20;
    [SerializeField] private float segmentLength = 0.5f;
    [SerializeField] private float segmentRadius = 0.05f;
    [SerializeField] private float segmentMass = 0.3f;

    [Header("Anchors")]
    [SerializeField] private Transform startAnchor;
    [SerializeField] private bool anchorStart = true;

    [Header("Rendering")]
    [SerializeField] private Material ropeMaterial;

    private readonly List<Rigidbody> bodies = new List<Rigidbody>();
    private LineRenderer lineRenderer;

    private void Start()
    {
        BuildRope();
    }

    public void BuildRope()
    {
        Clear();

        Vector3 startPos = startAnchor ? startAnchor.position : transform.position;
        Rigidbody previousBody = null;

        for (int i = 0; i < segmentCount; i++)
        {
            // Создаем сегмент
            GameObject seg = new GameObject($"RopeSegment_{i}");
            seg.transform.parent = transform;
            seg.transform.position = startPos + Vector3.down * segmentLength * i;

            // Добавляем коллайдер
            CapsuleCollider col = seg.AddComponent<CapsuleCollider>();
            col.radius = segmentRadius;
            col.height = segmentLength;
            col.direction = 1;

            // Добавляем Rigidbody
            Rigidbody rb = seg.AddComponent<Rigidbody>();
            rb.mass = segmentMass;
            rb.drag = 0.2f;
            rb.angularDrag = 0.05f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            if (i == 0)
            {
                rb.isKinematic = true;
            }

            bodies.Add(rb);

            // Добавляем соединение с предыдущим сегментом
            if (previousBody != null)
            {
                CharacterJoint joint = seg.AddComponent<CharacterJoint>();
                joint.connectedBody = previousBody;
                joint.enablePreprocessing = true;

                SoftJointLimit limit = new SoftJointLimit { limit = 25f };
                joint.lowTwistLimit = limit;
                joint.highTwistLimit = limit;
                joint.swing1Limit = limit;
                joint.swing2Limit = limit;
            }

            previousBody = rb;
        }

        // Привязка к стартовому якорю
        if (anchorStart && startAnchor != null)
        {
            Rigidbody anchorRb = startAnchor.GetComponent<Rigidbody>();
            if (!anchorRb)
            {
                anchorRb = startAnchor.gameObject.AddComponent<Rigidbody>();
                anchorRb.isKinematic = true;
            }

            CharacterJoint anchorJoint = bodies[0].gameObject.AddComponent<CharacterJoint>();
            anchorJoint.connectedBody = anchorRb;
            anchorJoint.enablePreprocessing = true;

            SoftJointLimit lockLimit = new SoftJointLimit { limit = 0f };
            anchorJoint.lowTwistLimit = lockLimit;
            anchorJoint.highTwistLimit = lockLimit;
            anchorJoint.swing1Limit = lockLimit;
            anchorJoint.swing2Limit = lockLimit;
        }

        IgnoreSelfCollisions();
        SetupLineRenderer();
    }

    private void IgnoreSelfCollisions()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();
        for (int i = 0; i < cols.Length; i++)
            for (int j = i + 1; j < cols.Length; j++)
                Physics.IgnoreCollision(cols[i], cols[j]);
    }

    private void SetupLineRenderer()
    {
        // Получаем или создаем LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = bodies.Count;
        lineRenderer.startWidth = segmentRadius * 2f;
        lineRenderer.endWidth = segmentRadius * 2f;

        if (ropeMaterial != null)
            lineRenderer.material = ropeMaterial;
        else
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void Update()
    {
        if (lineRenderer == null || bodies.Count == 0) return;

        for (int i = 0; i < bodies.Count; i++)
        {
            if (bodies[i])
                lineRenderer.SetPosition(i, bodies[i].position);
        }
    }

    private void Clear()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        bodies.Clear();

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        segmentCount = Mathf.Max(2, segmentCount);
        segmentLength = Mathf.Max(0.05f, segmentLength);
        segmentRadius = Mathf.Max(0.01f, segmentRadius);
        segmentMass = Mathf.Max(0.05f, segmentMass);
    }
#endif
}
