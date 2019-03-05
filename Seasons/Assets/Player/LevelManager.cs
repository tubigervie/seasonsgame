using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] GameObject startPoint;
    public static LevelManager singleton;
    public GameObject currentCheckpoint;
    Player player;
    public List<GrappleObject> grappleObjects = new List<GrappleObject>();

    private void Awake()
    {
        singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = Player.singleton;
        currentCheckpoint = startPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DieRespawn()
    {
        CameraManager.singleton.enabled = false;
        yield return new WaitForSeconds(2f);
        player.transform.position = currentCheckpoint.transform.position;
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
