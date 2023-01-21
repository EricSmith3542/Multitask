using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource mainMenuMusicSource;
    [SerializeField] private AudioSource[] musicSources;
    [SerializeField] private AudioSource effectSource;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartMainMenuMusic()
    {
        mainMenuMusicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        effectSource.PlayOneShot(clip);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ChangeEffectsVolume(float value)
    {
        effectSource.volume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        mainMenuMusicSource.volume = value;
        foreach (AudioSource musicSource in musicSources)
        {
            musicSource.volume = value;
        }
    }

    public void StartMusicSourceByIndex(int index)
    {
        musicSources[index].Play();
    }

    public void StartAllMusicUpToIndex(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            musicSources[i].Play();
        }
    }

    public void PlayMusicWithoutLoop(int index)
    {
        musicSources[index].PlayOneShot(musicSources[index].clip);
    }

    public void StopAllMusic()
    {
        mainMenuMusicSource.Stop();
        foreach (AudioSource musicSource in musicSources)
        {
            musicSource.Stop();
        }
    }

    public float SecondsUntilMusicLoop(int index)
    {
        return musicSources[index].clip.length - musicSources[index].time;
    }

    public float GetMusicSecondsByIndex(int index)
    {
        return musicSources[index].clip.length;
    }

    public void ChangePitch(float pitch)
    {
        effectSource.pitch = pitch;
        foreach (AudioSource musicSource in musicSources)
        {
            musicSource.pitch = pitch;
        }
    }
}
