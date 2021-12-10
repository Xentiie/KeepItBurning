using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * Simple script that makes the camera follow the player smoothly
 * 
 */
public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    [Space]
    public float scrollSpeed = 1;
    public float camOffsetBack;

    private Camera cam;

    private void Start() {
        cam = GetComponent<Camera>();
    }

    private void FixedUpdate() {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void Update() {
        float scrollPower = Input.GetAxis("Mouse ScrollWheel");
        float newSize = cam.orthographicSize + scrollPower * scrollSpeed;
        newSize = Mathf.Clamp(newSize, camOffsetBack+1, camOffsetBack+5);
        cam.orthographicSize = newSize;
    }

}
