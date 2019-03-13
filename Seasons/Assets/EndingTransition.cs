using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingTransition : MonoBehaviour
{
    public string load_scene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObject.FindGameObjectsWithTag("!EndingTree").Length == 0)
        {
            if (collision.gameObject.CompareTag("EndingTree"))
            {
                MusicManager.singleton.isPlayingLevelTrack = false;
                GameManager.singleton.StartCoroutine(GameManager.singleton.FadeInToScene(load_scene));
            }
        }
       
    }
}
