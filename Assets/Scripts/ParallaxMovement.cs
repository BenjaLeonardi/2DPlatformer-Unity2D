using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed;
    private float startPos;
    private GameObject myCamera;
    private float spriteLength;
    // Start is called before the first frame update
    void Start()
    {
        myCamera = GameObject.Find("CameraBackUp");
        startPos = transform.position.x;
        spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float temporalPos = (myCamera.transform.position.x * (1 - parallaxSpeed));
        float distanceToMove = (myCamera.transform.position.x * parallaxSpeed);

        transform.position = new Vector3(startPos + distanceToMove, transform.position.y, transform.position.z);

        if (temporalPos > startPos + spriteLength)
        {
            startPos += spriteLength;
        }
        else if (temporalPos < startPos - spriteLength)
        {
            startPos -= spriteLength;
        }
    }
}
