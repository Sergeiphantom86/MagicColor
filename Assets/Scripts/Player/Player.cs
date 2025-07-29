using Cinemachine;
using UnityEngine;
using YG;

[RequireComponent(typeof(GameSaveSystem), typeof(Collider))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CameraController _cameraController;

    private Collider _collider;
    private GameSaveSystem _saveSystem;
    private Sprite _sprite;


    public Sprite Sprite { get; private set; }

    private void Awake()
    {
        _controller = GetComponentInChildren<PlayerController>();
        _collider = GetComponent<Collider>();
        _saveSystem = GetComponent<GameSaveSystem>();

        TurnOnMovement();
    }

    private void Start()
    {
        LoadPlayerPosition();
    }

    public void TurnOffMovement()
    {
        if (_controller == null) return;
        _controller.TurnOffMovement();
    }

    public void TurnOnMovement()
    {
        if (_controller == null) return;
        _controller.TurnOnMovement();
    }

    public void UpdateSprite(Sprite newSprite)
    {
        if (newSprite == null) return;
        if (_sprite != null) return;

        _sprite = newSprite;
    }

    public Sprite GetSprite() => 
        _sprite;

    public void ResetSprite() => 
        _sprite = null;

    public void ColliderEnabled(bool isOn)
    {
        _collider.enabled = isOn;
    }

    public void TurnOff()
    {
        _playerMovement.TurnOff();
    }

    public void TurnOn()
    {
        _playerMovement.TurnOn();
    }

    public void ResetPath()
    {
        _playerMovement.ResetPath();
    }

    private void LoadPlayerPosition()
    {
        if (_saveSystem != null)
        {
           
        }
        else
        {
            Debug.LogWarning("GameSaveSystem not found on Player!");
        }
    }
}