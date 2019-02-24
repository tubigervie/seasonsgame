using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillar : MonoBehaviour, IFlammable
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    BoxCollider2D collider;

    public float projectileSpeed = 5f;
    public void Init(bool isLeft = false)
    {
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        collider.offset = (isLeft) ? new Vector2(collider.offset.x * -1, 0) : collider.offset;
        sprite.flipX = isLeft;
        rigid = GetComponent<Rigidbody2D>();
        if (isLeft)
            rigid.velocity = (-transform.right * projectileSpeed);
        else
            rigid.velocity = transform.right * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && rigid.bodyType != RigidbodyType2D.Static)
        {
            if(other.gameObject.GetComponent<IcePillar>() == null)
            {
                rigid.velocity = Vector2.zero;
                BoxCollider2D collider = GetComponent<BoxCollider2D>();
                collider.isTrigger = false;
                rigid.bodyType = RigidbodyType2D.Static;
                Destroy(this.gameObject, 10f);
            }
        }
    }

    public void Burn()
    {
        Destroy(this.gameObject);
    }
}
