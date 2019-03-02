using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleObject : MonoBehaviour
{
    public float timer;
    public bool canGrapple = true;

    // Update is called once per frame
    void Update()
    {
        if(!canGrapple)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                canGrapple = true;
                timer = 0;
            }
        }
    }
}
