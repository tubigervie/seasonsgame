using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCone : MonoBehaviour
{
    public SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        IFlammable canBurn = collision.GetComponent<IFlammable>();
        if (canBurn != null)
            canBurn.Burn();
    }
}
