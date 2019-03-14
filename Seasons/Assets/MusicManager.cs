using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource music;
    public static MusicManager singleton;
    public bool isPlayingLevelTrack;

    private void Awake()
    {
        if (singleton == null)
        {
            DontDestroyOnLoad(this.gameObject);
            singleton = this;
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        music = GetComponent<AudioSource>();
    }

    public void PlayTrack(AudioClip track)
    {
        music.Stop();
        music.clip = track;
        music.Play();
    }

    public IEnumerator FadeInTrack(float levelVolume)
    {
        music.Stop();
        music.Play();
        music.volume = 0;
        for (float i = 0; i <= levelVolume; i += Time.deltaTime / 2f)
        {
            // set color with i as alpha
            music.volume += Time.deltaTime / 2f;
            yield return null;
        }
    }
}
