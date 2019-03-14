using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroCutscene : MonoBehaviour
{
    double currentTime;
    [SerializeField] double videoTime = .44f;
    VideoPlayer video;
    // Start is called before the first frame update
    void Start()
    {
        video = GetComponent<VideoPlayer>();
        videoTime = video.clip.length;
        //MusicManager.singleton.StartCoroutine(MusicManager.singleton.FadeInTrack(.2f));
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if(!(currentTime < videoTime))
        {
            GameManager.singleton.loadTutorial();
        }
        
    }
}
