using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public PlayerController playerTarget;
    public Vector2 focusAreaSize;
    public float verticalOffset;
    public float lookAheadX;
    public float lookSmoothXTime;
    public float verticalSmoothTime;

    FocusArea focusArea;
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;
    bool lookAheadStop;

    public static CameraManager singleton;

    private void Awake()
    {
        singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        focusArea = new FocusArea(playerTarget.GetComponent<BoxCollider2D>().bounds, focusAreaSize);    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        focusArea.UpdateCameraBounds(playerTarget.GetComponent<BoxCollider2D>().bounds);
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            if(Mathf.Sign(playerTarget.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && playerTarget.playerInput.x != 0)
            {
                lookAheadStop = false;
                targetLookAheadX = lookAheadDirX * lookAheadX;
            }
            else
            {
                if(!lookAheadStop)
                {
                    lookAheadStop = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadX - currentLookAheadX) / 4f; //gives us a fraction of how far we need to go to complete the look ahead
                }
            }
        }
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothXTime);
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left;
        float right;
        float top;
        float bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = Vector2.zero;
        }

        public void UpdateCameraBounds(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
                shiftX = targetBounds.min.x - left;
            else if (targetBounds.max.x > right)
                shiftX = targetBounds.max.x - right;
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
                shiftY = targetBounds.min.y - bottom;
            else if (targetBounds.max.y > top)
                shiftY = targetBounds.max.y - top;
            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }
}
