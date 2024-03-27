using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform playerPos;
    public float smoothSpeed;
    public Vector3 offset;
    public float _y;
    public float maxDistance;
    private Vector3 velocity = Vector3.zero;
    
    void Start()
    {
        gameObject.transform.position = playerPos.position;
    }

    void FixedUpdate()
    {
        Vector3 targetPosition = playerPos.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }
}
