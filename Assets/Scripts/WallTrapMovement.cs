using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTrapMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(transform.position.x + (movementSpeed * Time.deltaTime), transform.position.y);
    }
}
