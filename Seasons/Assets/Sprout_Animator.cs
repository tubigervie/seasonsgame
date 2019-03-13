using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprout_Animator : MonoBehaviour
{
    public static bool gameIsPaused = false;
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
        while (time < 1.0 && Time.timeScale > 0)
        {

            time += Time.deltaTime * rate;
            this.transform.localScale += new Vector3(0.007f, 0.007f, 0);
            yield return null;

        }
    }

    private IEnumerator Growing()
    {
        var time = 0.0;
        while (time < 1.0 && Time.timeScale > 0)
        {
           
            time += Time.deltaTime * rate;
            this.transform.position += new Vector3(0f, 0.005f, 0);
            yield return null;

        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        Time.timeScale = 0f;
        gameIsPaused = true;
    }
}
