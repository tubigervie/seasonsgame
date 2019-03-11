using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource soundEffects;
    public static AudioManager singleton;

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        soundEffects = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(AudioClip sound, float volume)
    {
        soundEffects.PlayOneShot(sound, volume);
        Debug.Log("should play");
    }

    public void PlayLoopSoundEffect(AudioClip sound)
    {
        soundEffects.loop = true;
        soundEffects.clip = sound;
        soundEffects.Play();
    }

    public void TurnOffLoop()
    {
        soundEffects.Stop();
        soundEffects.clip = null;
        soundEffects.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
