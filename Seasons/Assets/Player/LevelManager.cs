using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager singleton;
    public GameObject currentCheckpoint;
    Player player;

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Player.singleton;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DieRespawn()
    {
        CameraManager.singleton.enabled = false;
        yield return new WaitForSeconds(2f);
        player.transform.position = currentCheckpoint.transform.position;
        CameraManager.singleton.enabled = true;
    }
}
