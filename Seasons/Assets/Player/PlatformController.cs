using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : Raycaster
{
    public LayerMask passengerMask;
    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public float speed;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;
    public bool cyclic;
    int fromWaypointIndex;
    float valueBetweenWaypoints;
    float _waitTime;

    List<PassengerMovement> passengerMoveList;
    Dictionary<Transform, PlayerController> passengerDict = new Dictionary<Transform, PlayerController>();
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; ++i)
            globalWaypoints[i] = localWaypoints[i] + transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRaycastTargets();
        Vector3 velocity = PlatformMovement();
        CalculatePassengerMovement(velocity);
        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    Vector3 PlatformMovement()
    {
        if (Time.time < _waitTime)
            return Vector3.zero;

        fromWaypointIndex %= globalWaypoints.Length;

        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        valueBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints; //divide speed by distance so that the percentage does not increase faster the farther waypoints are
        valueBetweenWaypoints = Mathf.Clamp01(valueBetweenWaypoints);
        float easedValueBetweenWaypoints = EaseMovement(valueBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedValueBetweenWaypoints);

        if(valueBetweenWaypoints >= 1)
        {
            valueBetweenWaypoints = 0;
            fromWaypointIndex++;
            if(!cyclic)
            {
                if (fromWaypointIndex >= globalWaypoints.Length - 1)
                {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            _waitTime = Time.time + waitTime;
        }
        return newPos - transform.position;
    }

    float EaseMovement(float x)
    {
        float a = easeAmount + 1; 
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a)); //provides acceleration and deceleration in between movements simulating a movement curve
    }

    void MovePassengers(bool moveBeforePlatform)
    {
        foreach(PassengerMovement passenger in passengerMoveList)
        {
            if (!passengerDict.ContainsKey(passenger.transform))
                passengerDict.Add(passenger.transform, passenger.transform.GetComponent<PlayerController>());
            if(passenger.moveBeforePlatform == moveBeforePlatform)
                passengerDict[passenger.transform].Move(passenger.velocity, passenger.isOnPlatform);
        }
    }



    //anything being moved by platform is a passenger
    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>(); //really good at quickly adding and checking for things
        passengerMoveList = new List<PassengerMovement>();
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //Vertically moving platform
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastTargets.bottomLeft : raycastTargets.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);
                Physics2D.SyncTransforms();
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                if(hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY; //distance between the player and the platform 

                        passengerMoveList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                    }
                }
            }
        }
        //Horizontally moving platform
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastTargets.bottomLeft : raycastTargets.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);
                Physics2D.SyncTransforms();
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;
                        passengerMoveList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                    }
                }
            }
        }
        //Horizontally moving platform or downward moving platform
        if(directionY == -1 || velocity.y == 0 && velocity.x != 0) //change to -1
        {
            float rayLength = skinWidth * 2;
            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastTargets.topLeft + Vector2.right * (verticalRaySpacing * i);
                Physics2D.SyncTransforms();
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                if (hit)
                {
                    if (!movedPassengers.Contains(hit.transform))
                    {
                        movedPassengers.Add(hit.transform);
                        float pushX = velocity.x;
                        float pushY = velocity.y; //distance between the player and the platform 
                        passengerMoveList.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                    }
                }
            }
        }
    }

    struct PassengerMovement
    {
        public Transform transform;
        public Vector3 velocity;
        public bool isOnPlatform;
        public bool moveBeforePlatform;

        public PassengerMovement(Transform _transform, Vector3 _velocity, bool _isOnPlatform, bool _moveBeforePlayform)
        {
            transform = _transform;
            velocity = _velocity;
            moveBeforePlatform = _moveBeforePlayform;
            isOnPlatform = _isOnPlatform;
        }
    }

    private void OnDrawGizmos()
    {
        if(localWaypoints != null)
        {
            Gizmos.color = Color.cyan;
            float size = .3f;
            for (int i = 0; i < localWaypoints.Length; ++i)
            {
                Vector3 globalWaypointPos = (Application.isPlaying)? globalWaypoints[i] : localWaypoints[i] + transform.position; //if playing, use global waypoints otherwise use waypoints local to moving platform
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
