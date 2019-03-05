using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnGate : MonoBehaviour, IFlammable
{
    [SerializeField] int health = 5;
    float timer;
    
    public void Burn()
    {
        if (timer > 0)
            return;
        health--;
        timer = .3f;
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
