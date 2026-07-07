using Menu.Tutorials;
using PuzzleEditor.MinigamesRoulette;
using PuzzleEditor.Audio;
using System;
using System.Collections;
using UnityEngine;

namespace PuzzleEditor.LockMechanics
{
    [RequireComponent(typeof(Unblocker), typeof(Oscillator), typeof(Voiceover))]

    public class Lock : MonoBehaviour
    {
        [SerializeField] private AudioClip _flight;
        [SerializeField] private AudioClip _blocking;
        [SerializeField] private ErrorPanel _errorPanel;

        private Oscillator _ocillator;
        private Unblocker _unblocker;
        private Voiceover _voiceover;
        private LockPointer[] _lockPointers;
        private Collider _collider;
        private WaitForSeconds _waitFlightSoundFinish;
        private bool _isUsed;

        public event Action Unblocking;

        public bool IsUsed => _isUsed;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _unblocker = GetComponent<Unblocker>();
            _voiceover = GetComponent<Voiceover>();
            _ocillator = GetComponent<Oscillator>();
            _lockPointers = GetComponentsInChildren<LockPointer>();
            _waitFlightSoundFinish = new WaitForSeconds(_flight.length);

            if (_ocillator == null)
            {
                Debug.LogError("Oscillator = null");
                return;
            }

            if (_unblocker == null)
            {
                Debug.LogError("Unblocker = null");
                return;
            }

            if (_lockPointers == null)
            {
                Debug.LogError("LockPointer = null");
                return;
            }

            if (_voiceover == null)
            {
                Debug.LogError("Voiceover = null");
                return;
            }

            if (_collider == null)
            {
                Debug.LogError("Collider = null");
                return;
            }
        }

        public void SetUsed(bool isUsed)
        {
            _isUsed = isUsed;
        }

        public void SetAngle(Vector3 angleRotation)
        {
            transform.Rotate(angleRotation);
        }

        public void Unblock()
        {
            _unblocker.Play();
            _voiceover.PlayOneShot(_flight);

            _collider.enabled = false;

            SetColor();

            Unblocking?.Invoke();

            StartCoroutine(WaitTurnOff());
        }

        public void ShakeUp()
        {
            if (_errorPanel != null)
            {
                _errorPanel.TurnOn();
            }

            _ocillator.Play();
            _voiceover.PlayOneShot(_blocking);
        }

        private void SetColor()
        {
            foreach (LockPointer lockPointer in _lockPointers)
            {
                lockPointer.SetColor();
            }
        }

        private IEnumerator WaitTurnOff()
        {
            yield return _waitFlightSoundFinish;

            gameObject.SetActive(false);
        }
    }
}