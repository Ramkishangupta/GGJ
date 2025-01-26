using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;  // Singleton instance

    public AudioSource bgmSource;         // Source for background music
    public AudioSource sfxSource;         // Source for sound effects
    public AudioSource uiSource;          // Source for UI sounds

    public AudioClip[] bgmClips;          // Array of background music clips
    public AudioClip[] sfxClips;          // Array of sound effects clips
    public AudioClip[] uiClips;           // Array of UI sounds clips

    void Awake()
    {
        // Make sure there's only one instance of the AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this instance alive across scenes
        }
        else
        {
            Destroy(gameObject);           // Destroy any extra AudioManager instances
        }
    }

    void Start(){
        PlayBGM(0);
    }

    // Function to play background music
    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    // Function to play a sound effect
    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[index]);
        }
    }

    // Function to play a UI sound
    public void PlayUISound(int index)
    {
        if (index >= 0 && index < uiClips.Length)
        {
            uiSource.PlayOneShot(uiClips[index]);
        }
    }

    // Function to stop background music
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // Function to adjust BGM volume
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    // Function to adjust SFX volume
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    // Function to adjust UI sound volume
    public void SetUIVolume(float volume)
    {
        uiSource.volume = volume;
    }
}
