using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] 
public class GrappleObject : MonoBehaviour
{
    public Color color = Color.yellow;
    public float timer;
    public bool canGrapple = true;
    public bool isHighlighted = false;

    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    public void UpdateOutline(bool turnOn)
    {
        MaterialPropertyBlock mbp = new MaterialPropertyBlock();
        sprite.GetPropertyBlock(mbp);
        mbp.SetFloat("_Outline", turnOn ? 1f : 0);
        mbp.SetColor("_OutlineColor", color);
        sprite.SetPropertyBlock(mbp);
    }

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
