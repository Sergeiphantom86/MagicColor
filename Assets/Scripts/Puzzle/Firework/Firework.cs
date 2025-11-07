using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    [System.Serializable]
    public class SoundEvent
    {
        public string triggerTag = "Explosion";
        public AudioClip sound;
        public float volume = 1f;
    }

    [SerializeField] private SoundEvent[] soundEvents;
    [SerializeField] private ParticleSystem fireworksSystem;

    private AudioSource _audioSource;
    private ParticleSystem.TriggerModule _triggerModule;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.spatialBlend = 1f; // 3D звук
        }

        if (fireworksSystem != null)
        {
            _triggerModule = fireworksSystem.trigger;
            _triggerModule.enabled = true;
        }
    }

    private void OnParticleTrigger()
    {
        if (fireworksSystem == null) return;
        Debug.Log(10);
        // Получаем частицы, которые сработали на триггере
        List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle> ();

        int numEnter = fireworksSystem.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);

        for (int i = 0; i < numEnter; i++)
        {
            PlayRandomExplosionSound();
        }

        fireworksSystem.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, particles);
    }

    private void PlayRandomExplosionSound()
    {
        if (soundEvents.Length > 0)
        {
            SoundEvent randomEvent = soundEvents[Random.Range(0, soundEvents.Length)];
            if (randomEvent.sound != null)
            {
                _audioSource.PlayOneShot(randomEvent.sound, randomEvent.volume);
            }
        }
    }
}