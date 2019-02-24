using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public LayerMask collisionMask;
    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float maxClimbAngle = 80;
    float maxDescendAngle = 75;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    public Rigidbody2D rigid;
    RaycastTargets raycastTargets;
    [SerializeField] public CollisionInfo collisions;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastTargets.bottomLeft : raycastTargets.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if(collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlopeStart = 0;
                    if(slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }
                if(!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if(collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        if (Player.singleton.vineGrapplePointObject != null)
            return;
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if(velocity.y > climbVelocityY)
        {
            Debug.Log("Jumping on slope!");
        }
        else
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        if (Player.singleton.vineGrapplePointObject != null)
            return;
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastTargets.bottomRight : raycastTargets.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if(hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if(Mathf.Sign(hit.normal.x) == directionX)
                {
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle) * Mathf.Deg2Rad * Mathf.Abs(velocity.x))
                    {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastTargets.bottomLeft : raycastTargets.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if(hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if(collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }

            if (collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + skinWidth;
                Vector2 ray2Origin = ((directionX == -1) ? raycastTargets.bottomLeft : raycastTargets.topLeft) + Vector2.up * velocity.y;
                RaycastHit2D hit2 = Physics2D.Raycast(ray2Origin, Vector2.right * directionX, rayLength, collisionMask);

                if(hit2)
                {
                    float slopeAngle = Vector2.Angle(hit2.normal, Vector2.up);
                    if(slopeAngle != collisions.slopeAngle)
                    {
                        velocity.x = (hit.distance - skinWidth) * directionX;
                        collisions.slopeAngle = slopeAngle;
                    }
                }
            }

        }
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastTargets();
        collisions.Reset();
        collisions.velocityOld = velocity;

        if(velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if(velocity.x != 0)
            HorizontalCollisions(ref velocity);
        if(velocity.y != 0)
            VerticalCollisions(ref velocity);
        transform.Translate(velocity);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void UpdateRaycastTargets()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastTargets.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastTargets.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastTargets.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastTargets.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    struct RaycastTargets
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above;
        public bool below;
        public bool left;
        public bool right;

        public bool descendingSlope;
        public bool climbingSlope;
        public float slopeAngle;
        public float slopeAngleOld;

        public Vector3 velocityOld;


        public void Reset()
        {
            above = false;
            below = false;
            left = false;
            right = false;
            climbingSlope = false;
            descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
}
