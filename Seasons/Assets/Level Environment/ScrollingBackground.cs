using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public bool scrolling, parallax;
    public float backgroundSize;

    private Transform cameraTransform;
    [SerializeField] Transform[] layers;
    private float viewZone = 6;
    int leftIndex;
    int rightIndex;
    private float lastCameraX;

    public float parallaxSpeed;


    private void Start()
    {
        cameraTransform = Camera.main.transform;
        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }


    private void Update()
    {
        if(parallax)
        {
            float deltaX = cameraTransform.position.x - lastCameraX;
            Vector3 targetPos = transform.position;
            targetPos.x += deltaX * parallaxSpeed;
            transform.position = targetPos;
            lastCameraX = cameraTransform.position.x;
        }

        if(scrolling)
        {
            if (cameraTransform.position.x < (layers[leftIndex].transform.position.x + viewZone))
                ScrollLeft();
            if (cameraTransform.position.x > (layers[rightIndex].transform.position.x - viewZone))
                ScrollRight();
        }
    }

    private void ScrollLeft()
    {
        int lastRight = rightIndex;
        Vector3 targetPos = layers[rightIndex].position;
        targetPos.x = layers[leftIndex].position.x - backgroundSize;
        layers[rightIndex].position = targetPos;
        leftIndex = rightIndex;
        rightIndex--;
        if (rightIndex < 0)
            rightIndex = layers.Length - 1;
    }

    private void ScrollRight()
    {
        int lastLeft = leftIndex;
        Vector3 targetPos = layers[leftIndex].position;
        targetPos.x = layers[rightIndex].position.x + backgroundSize;
        layers[leftIndex].position = targetPos;
        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
            leftIndex = 0;
    }
}
