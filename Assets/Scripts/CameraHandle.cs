using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandle : MonoBehaviour
{
    [Header("Offsets")]
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [Space]
    [SerializeField] private Transform playerTransform;
    private void Update()
    {
        transform.position = new Vector3(playerTransform.position.x + offsetX, playerTransform.position.y + offsetY, transform.position.z);
    }
}
