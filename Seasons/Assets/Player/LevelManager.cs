﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    public static LevelManager singleton;
    public GameObject currentCheckpoint;
    Player player;
    public List<GrappleObject> grappleObjects = new List<GrappleObject>();
    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip levelOST;
    [SerializeField] float levelVolume = .1f;

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.gameIsPaused = true;
        player = Player.singleton;
        currentCheckpoint = startPoint;
        if (!MusicManager.singleton.isPlayingLevelTrack)
            StartCoroutine("PlayLevelMusic");
        else
            PauseMenu.gameIsPaused = false;
    }

    private IEnumerator PlayLevelMusic()
    {
        yield return new WaitForSeconds(.5f);
        MusicManager.singleton.PlayTrack(levelOST);
        MusicManager.singleton.StartCoroutine(MusicManager.singleton.FadeInTrack(levelVolume));
        MusicManager.singleton.isPlayingLevelTrack = true;
        PauseMenu.gameIsPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DieRespawn()
    {
        player.ResetAbilities();
        AudioManager.singleton.TurnOffLoop();
        AudioManager.singleton.PlaySoundEffect(deathSFX, .3f);
        player.velocity = Vector3.zero;
        player.gameObject.SetActive(false);
        CameraManager.singleton.enabled = false;
        yield return new WaitForSeconds(.5f);
        player.transform.position = currentCheckpoint.transform.position;
        player.gameObject.SetActive(true);
        CameraManager.singleton.enabled = true;
    }

    public GrappleObject NextGrappleObject(Vector3 position, float minDistance)
    {
        float min = minDistance;
        GrappleObject closestTarget = null;
        for(int i = 0; i < grappleObjects.Count; i++)
        {
            float distance = Vector3.Distance(position, grappleObjects[i].transform.position);
            if (grappleObjects[i].canGrapple && distance < min)
            {
                min = distance;
                closestTarget = grappleObjects[i];
            }
        }
        return closestTarget;
    }

    public void HighlightGrappleObjects(Vector3 position, float minDistance)
    {
        float min = minDistance;
        for (int i = 0; i < grappleObjects.Count; i++)
        {
            float distance = Vector3.Distance(position, grappleObjects[i].transform.position);
            if (grappleObjects[i].canGrapple && distance < min && !grappleObjects[i].isHighlighted)
            {
                min = distance;
                grappleObjects[i].UpdateOutline(true);
                grappleObjects[i].isHighlighted = true;
            }
            else if ((!grappleObjects[i].canGrapple || distance > min) && grappleObjects[i].isHighlighted)
            {
                grappleObjects[i].UpdateOutline(false);
                grappleObjects[i].isHighlighted = false;
            }
        }
    }
}
