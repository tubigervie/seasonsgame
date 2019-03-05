using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    public static LevelManager singleton;
    public GameObject currentCheckpoint;
    Player player;
    public List<GrappleObject> grappleObjects = new List<GrappleObject>();
    public Text health;

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Player.singleton;
        currentCheckpoint = startPoint;
        health.text = "3";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DieRespawn()
    {
        int i = int.Parse(health.text); i--;
        health.text = i.ToString();
        player.ResetAbilities();
        player.velocity = Vector3.zero;
        player.gameObject.SetActive(false);
        CameraManager.singleton.enabled = false;
        yield return new WaitForSeconds(.5f);
        player.transform.position = currentCheckpoint.transform.position;
        player.gameObject.SetActive(true);
        CameraManager.singleton.enabled = true;
    }

    public GrappleObject NextGrappleObject(Vector3 position, float minDistance)
    {
        float min = minDistance;
        GrappleObject closestTarget = null;
        for(int i = 0; i < grappleObjects.Count; i++)
        {
            float distance = Vector3.Distance(position, grappleObjects[i].transform.position);
            if (grappleObjects[i].canGrapple && distance < min)
            {
                min = distance;
                closestTarget = grappleObjects[i];
            }
        }
        return closestTarget;
    }
}
