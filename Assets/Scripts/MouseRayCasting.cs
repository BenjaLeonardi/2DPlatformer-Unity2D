using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseRayCasting : MonoBehaviour
{
    private LineRenderer liner;
    void Start()
    {
        liner = GetComponent<LineRenderer>();
    }
    void Update()
    {
        // Get the position of the mouse
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Create a ray from the mouse position
        Ray2D ray = new Ray2D(mousePosition, Vector2.zero);

        // Cast the ray and get information about the hit object
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // Draw a line representing the raycast
        Debug.DrawLine(ray.origin, hit.point, Color.red);

        // Draw a line representing the raycast
        liner.SetPosition(0, ray.origin);
        liner.SetPosition(1, hit.point);
        liner.widthMultiplier = 0.1f;

        // If the ray hit something
        if (hit.collider != null)
        {
            // Do something with the hit object, such as destroy it or damage it
        }
    }
}
