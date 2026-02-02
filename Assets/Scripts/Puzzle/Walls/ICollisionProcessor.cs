using UnityEngine;

public interface ICollisionProcessor
{
    void ProcessEnter(Collider other);
    void ProcessExit(Collider other);
}