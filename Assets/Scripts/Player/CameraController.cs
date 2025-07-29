using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private CinemachineVirtualCamera _playerCamera; 
    [SerializeField] private CinemachineVirtualCamera _dialogueCamera; 
   
    public void StartDialogueView(Transform npc)
    {
        SetPriority(20);
    }

    public void EndDialogueView()
    {
        SetPriority(0);
    }

    private void SetPriority(int fromPriority)
    {
        _dialogueCamera.Priority = fromPriority;
    }
}