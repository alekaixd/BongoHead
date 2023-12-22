using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    public float smoothSpeed;
    private Vector3 offset = new Vector3 (0, 0, -10);
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = playerPos.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = playerPos.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }
}
