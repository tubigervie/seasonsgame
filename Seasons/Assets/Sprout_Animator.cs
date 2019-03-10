using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprout_Animator : MonoBehaviour
{
    private float rate = 1f;

    private void Start()
    {
        this.transform.localScale = new Vector3(0.5f, 0.5f, 0);
    }
    // Start is called before the first frame update
    private void Awake()
    {
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 0.1f, this.transform.position.z);
        StartCoroutine(Scaling());
        StartCoroutine(Growing());
    }

    private IEnumerator Scaling()
    {
        var time = 0.0;
        while (time < 1.0)
        {
            time += Time.deltaTime * rate;
            this.transform.localScale += new Vector3(0.007f, 0.007f, 0);
            yield return null;

        }
    }

    private IEnumerator Growing()
    {
        var time = 0.0;
        while (time < 1.0)
        {
            time += Time.deltaTime * rate;
            this.transform.position += new Vector3(0f, 0.005f, 0);
            yield return null;

        }
    }
}
