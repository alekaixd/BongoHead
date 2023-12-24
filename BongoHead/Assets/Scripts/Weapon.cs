using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float rotationSpeed;
    public Player player;
    public Camera mainCamera;
    public float yOffset;
    public float xOffset;
    void Update()
    {
        PointToMouse();
    }

    private void PointToMouse()
    {
        var mouseScreenPos = Input.mousePosition;
        var startingScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        mouseScreenPos.x -= startingScreenPos.x;
        mouseScreenPos.y -= startingScreenPos.y;
        var angle = Mathf.Atan2(mouseScreenPos.y + yOffset, mouseScreenPos.x - xOffset) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        Debug.Log(angle);
    }

        
}
