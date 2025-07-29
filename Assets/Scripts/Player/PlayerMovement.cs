using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    public Vector3 Velocity => _agent.velocity;
    public float Speed => _agent.velocity.magnitude;
    public float Radius => _agent.radius;

    public void SetDestination(Vector3 position)
    {
        _agent.SetDestination(position);
    }

    public void Stop(bool isStopped)
    {
        _agent.isStopped = isStopped;
    }

    public void TurnOff()
    {
        _agent.enabled = false;
    }

    public void TurnOn()
    {
        _agent.enabled = true;
    }

    public void ResetPath()
    {
        _agent.ResetPath();
        _agent.velocity = Vector3.zero;

    }

    public bool TryGetNavMeshPosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit navHit, _agent.radius, NavMesh.AllAreas) == false)
        {
            return false;
        }

        return true;
    }
}