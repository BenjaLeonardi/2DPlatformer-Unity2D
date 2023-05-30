using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        //Podria usar un tag
        if (other.gameObject.name == "Player")
        {
            other.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.name == "Player")
        {
            other.gameObject.transform.SetParent(null);
        }
    }
}
