using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeActivator : MonoBehaviour
{
    private List<Transform> _transforms;
    private List<ColorableObject> _colorableObjects;
    private List<SmoothAppearance> _smoothAppearances;

    public List<Transform> Transforms => _transforms;

    private void Awake()
    {
        _transforms = new List<Transform>();
        _colorableObjects = new List<ColorableObject>();
        _smoothAppearances = new List<SmoothAppearance>();
    }

    private void Start()
    {
        _transforms = GetDirectChildren();
    }

    public void TurnOff(Color color)
    {
        StartCoroutine(WaitSpawn(color));
    }

    private List<Transform> GetDirectChildren()
    {
        List<Transform> children = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));

            _colorableObjects.Add(children[i].GetComponent<ColorableObject>());
            _smoothAppearances.Add(children[i].GetComponent<SmoothAppearance>());
        }

        return children;
    }

    private IEnumerator WaitSpawn(Color color)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return new WaitForSeconds(0.1f);

            _colorableObjects[i].SetColor(color);
            _smoothAppearances[i].Show();
        }
    }
}