using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnGate : MonoBehaviour, IFlammable
{
    [SerializeField] ParticleSystem burnGlow;
    [SerializeField] int health = 5;
    float timer;
    
    public void Burn()
    {
        if (timer > 0)
            return;
        health--;
        burnGlow.Play();
        timer = .7f;
        if(health <= 0)
            Destroy(this.gameObject);
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;
        }
    }
}
