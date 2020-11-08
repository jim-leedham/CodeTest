using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioMixerSnapshot bootSnapshot;
    [SerializeField] private AudioMixerSnapshot gameplaySnapshot;
    [SerializeField] private AudioSource sFXSource;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if(newState == GameState.BOOT)
        {
            bootSnapshot.TransitionTo(0.5f);
        }
        else
        {
            gameplaySnapshot.TransitionTo(0.5f);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sFXSource.clip = clip;
        sFXSource.Play();
    }
}
