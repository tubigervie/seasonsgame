using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = Color.red;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if(collision.gameObject.CompareTag("Player"))
        {
            LevelManager.singleton.currentCheckpoint = this.gameObject;
            sprite.color = Color.green;
        }
    }
}
