using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{

    //Se usa esto para pasar de una camara a otra cuando el jugador esta dentro del trigger del Room
    public GameObject virtualCam;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && !other.isTrigger){
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Player") && !other.isTrigger){
            virtualCam.SetActive(false);
        }
    }
}