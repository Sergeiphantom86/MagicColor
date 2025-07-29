using UnityEngine;

public class TablePlane : MonoBehaviour
{
    public Plane Plane { get; private set; }

    private void Awake()
    {
        UpdatePlane();
    }

    public void UpdatePlane()
    {
        Plane = new Plane(transform.up, transform.position);
    }
}