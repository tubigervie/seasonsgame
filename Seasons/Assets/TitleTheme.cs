using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTheme : MonoBehaviour
{
    [SerializeField] AudioClip titleTheme;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("PlayLevelMusic");
    }

    private IEnumerator PlayLevelMusic()
    {
        yield return new WaitForSeconds(.5f);
        MusicManager.singleton.PlayTrack(titleTheme);
        MusicManager.singleton.StartCoroutine(MusicManager.singleton.FadeInTrack(.4f));
        PauseMenu.gameIsPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
