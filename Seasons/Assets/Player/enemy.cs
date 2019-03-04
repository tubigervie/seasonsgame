using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed;
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, target.position) < 5)
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            speed = 0;
            StartCoroutine(Wait());
            LevelManager.singleton.StartCoroutine("DieRespawn");
        }
        if(collision.gameObject.name == "Fire")
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
        speed = 2;
    }
}
