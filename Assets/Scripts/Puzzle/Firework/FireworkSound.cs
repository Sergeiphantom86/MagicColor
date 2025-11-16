using System.Collections;
using UnityEngine;

public class FireworkSound : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private Voiceover _voiceover;
    private bool _isOn;

    private void Awake()
    {
        _voiceover = GetComponent<Voiceover>();
        _isOn = true;

        StartCoroutine(Hokvfov());
    }

    private IEnumerator Hokvfov()
    {
        while (_isOn)
        {
            yield return new WaitForSeconds(1);

            _voiceover.Play(_audioClip);
        }
    }
}