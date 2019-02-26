using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
	SpriteRenderer initial_state;
	SpriteRenderer end_state;
    // Start is called before the first frame update
    void Start()
    {
		initial_state = this.GetComponent<SpriteRenderer>();
		end_state = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
