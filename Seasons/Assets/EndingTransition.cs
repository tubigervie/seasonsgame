using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingTransition : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameObject.FindGameObjectsWithTag("!EndingTree").Length == 0)
        {
            if (collision.gameObject.CompareTag("EndingTree"))
            {
                StartCoroutine(GameObject.FindObjectOfType<SceneTransition>().FadeAndLoadScene(
                    SceneTransition.FadeDirection.Out, "Santiago 1"));
            }
        }
       
    }
}
