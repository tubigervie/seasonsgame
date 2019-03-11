using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillar : MonoBehaviour, IFlammable
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    PolygonCollider2D collider;
    ParticleSystem particleLoop;
    [SerializeField] AudioClip iceSFX;
    [SerializeField] GameObject particleBurst;
    [SerializeField] GameObject steamParticles;
    GameObject collidedObject;
    Vector3 collidedObjectInitialPosition;
    [SerializeField] [Range(0, 1)] float abilityVolume = .1f;
    bool isStatic;
    public float projectileSpeed = 10f;

    public void Init(bool isLeft = false)
    {
        GetComponent<AudioSource>().PlayOneShot(iceSFX, abilityVolume);
        particleLoop = GetComponentInChildren<ParticleSystem>();
        sprite = GetComponent<SpriteRenderer>();
        collider = GetComponent<PolygonCollider2D>();
        Vector3 targetRotation = transform.eulerAngles;
        targetRotation.y = (isLeft) ? 180 : 0;
        transform.eulerAngles = targetRotation;
        rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = transform.right * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 9 && rigid.bodyType != RigidbodyType2D.Static)
        {
            if(other.gameObject.GetComponent<IcePillar>() == null)
            {
                rigid.velocity = Vector2.zero;
                collider.isTrigger = false;
                collidedObject = other.gameObject;
                collidedObjectInitialPosition = collidedObject.transform.position;
                rigid.bodyType = RigidbodyType2D.Static;
                particleLoop.Stop();
                GetComponent<AudioSource>().PlayOneShot(iceSFX, abilityVolume);
                particleBurst.SetActive(true);
                Destroy(this.gameObject, 10f);
            }
        }
    }

    public void Burn()
    {
        var steam = Instantiate(steamParticles);
        steam.transform.position = transform.position;
        
        Destroy(this.gameObject, 1);
        Destroy(steam, 1.1f);
    }

    void Update()
    {
        if (collidedObject != null)
        {
            rigid.bodyType = RigidbodyType2D.Static;
            rigid.gravityScale = 1;
        }
        else
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
        if (collidedObject != null && collidedObject.transform.position != collidedObjectInitialPosition)
        {
            particleBurst.GetComponent<ParticleSystem>().Stop();
            particleBurst.GetComponent<ParticleSystem>().Play();
            Destroy(this.gameObject, .2f);
        }

    }
}
