using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class EndingTrees : MonoBehaviour
{

    //public Sprite masking_image;
    //private Texture2D stored_image;
   // public Player reference;
    //public GameObject thisobject;
    public float objective = 35f;
    private float progression = 0f;
    public Text restore;
    private Text info;
    

    // Start is called before the first frame update
    void Start()
    {
        info = restore.GetComponent<Text>();
        info.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Sprite crop = new Sprite();
        float percent = CalculatePercent();
        //Debug.Log(percent);
        if (percent >= 1)
        {
            Destroy(gameObject);
            Destroy(info);
        }

    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(2);
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           
            if (CalculatePercent() < 1)
            {
                info.enabled = true;
            }
        }
        else
        {
            info.enabled = false;
        }
        
    }

    private float CalculatePercent()
    {
        progression = 0;

        foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag("Element"))
        {
            progression += 1;
        }

        return progression / objective;
    }
}
