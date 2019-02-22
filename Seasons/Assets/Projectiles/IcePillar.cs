using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillar : MonoBehaviour
{
    Rigidbody2D rigid;
    public float projectileSpeed = 5f;
    public void Init(bool isLeft = false)
    {
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
}
