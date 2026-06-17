using System.Collections.Generic;
using UnityEngine;

public class FireworksController : MonoBehaviour
{
    private List<SimpleFireworksAudio> _allFireworksAudio;

    private void Awake()
    {
        _allFireworksAudio = new List<SimpleFireworksAudio>();

        GatherAllFireworksAudio();
    }

    public void Play()
    {
        foreach (var fireworks in _allFireworksAudio)
        {
            fireworks.StartFireworks();
        }
    }

    public void Stop()
    {
        foreach (var fireworks in _allFireworksAudio)
        {
            fireworks.Stop();
        }
    }

    private void GatherAllFireworksAudio()
    {
        _allFireworksAudio.Clear();
        _allFireworksAudio.AddRange(GetComponentsInChildren<SimpleFireworksAudio>());
    }
}