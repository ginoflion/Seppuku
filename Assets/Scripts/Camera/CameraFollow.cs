using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 10f; // Speed at which the camera follows the player
    public float yOffset = 3f; // Vertical offset for the player's position in the camera
    public Transform target; // Reference to the player (or target object)
    public BoxCollider2D cameraBounds; // BoxCollider2D that defines the camera boundaries

    private float minX, maxX, minY, maxY; // Boundaries of the camera movement
    private float camHalfHeight, camHalfWidth; // Half dimensions of the camera view in world units

    void Start()
    {
        Bounds bounds = cameraBounds.bounds;
        minX = bounds.min.x;
        maxX = bounds.max.x;
        minY = bounds.min.y;
        maxY = bounds.max.y;

        Camera cam = Camera.main;
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;
    }

    void Update()
    {
        // Calculate the target position with the vertical offset
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + yOffset, -10f);

        // Clamp the camera position to ensure it doesn't go outside the defined boundaries
        float clampedX = Mathf.Clamp(targetPos.x, minX + camHalfWidth, maxX - camHalfWidth);
        float clampedY = Mathf.Clamp(targetPos.y, minY + camHalfHeight, maxY - camHalfHeight);
        Vector3 clampedPos = new Vector3(clampedX, clampedY, -10f);

        // Smoothly move the camera to the target position
        transform.position = Vector3.Lerp(transform.position, clampedPos, followSpeed * Time.deltaTime);
    }
}
