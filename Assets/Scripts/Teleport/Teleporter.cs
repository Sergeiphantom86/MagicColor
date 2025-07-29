using Cinemachine;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(InteractionHandler))]
public class Teleporter : MonoBehaviour
{
    [SerializeField] protected PanelFader _fadePanel;
    [SerializeField] protected GameSaveSystem _gameSaveSystem;
    [SerializeField] protected TextDisplay _textDisplay;
    [SerializeField] protected Transform _defaultSpawnPoint;
    [SerializeField] protected CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] protected UISound _uISound;

    protected Player _player;
    protected InteractionHandler _interactionHandler;

    protected virtual void Awake()
    {
        _interactionHandler = GetComponent<InteractionHandler>();
    }

    private void OnEnable()
    {
        _interactionHandler.OnContact += OnPlayerContact;
    }

    private void OnDisable()
    {
        _interactionHandler.OnContact -= OnPlayerContact;
    }

    protected virtual void OnPlayerContact(Player player)
    {
        _player = player;
        player.TurnOffMovement();
        player.TurnOff();
        _uISound.OnTeleport();
    }

    protected IEnumerator TeleportPlayer(Vector3 targetPosition)
    {
        _player.ColliderEnabled(false);
        _fadePanel.Fade(1f, true);

        yield return new WaitForSeconds(_fadePanel.FadeDuration);

        _player.transform.position = targetPosition;

        SavePlayerPosition();

        RegainControl();
    }

    protected void RegainControl()
    {
        _fadePanel.Fade(0, false);

        _player.ColliderEnabled(true);
        _player.TurnOn();
        _player.ResetPath();
        _player.TurnOnMovement();
        _uISound.OnTeleport();
    }

    protected void SavePlayerPosition()
    {
        if (_gameSaveSystem != null)
        {
            
        }
        else
        {
            Debug.LogWarning("GameSaveSystem reference not set in Teleporter!");
        }
    }
}