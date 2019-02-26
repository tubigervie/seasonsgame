using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnGate : MonoBehaviour, IFlammable
{
    public void Burn()
    {
        Destroy(this.gameObject);
    }
}
