using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    protected BoxCollider2D collider;
    protected RaycastTargets raycastTargets;

    public LayerMask collisionMask;
    protected const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    protected Rigidbody2D rigid;

    public virtual void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    public virtual void Start()
    {
        CalculateRaySpacing();
    }

    public void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public void UpdateRaycastTargets()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastTargets.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastTargets.topRight = new Vector2(bounds.max.x, bounds.max.y);
        raycastTargets.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastTargets.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    public struct RaycastTargets
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }
}
