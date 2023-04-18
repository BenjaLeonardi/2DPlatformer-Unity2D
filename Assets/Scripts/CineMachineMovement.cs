using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CineMachineMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private CinemachineVirtualCamera virtualCam;
    private CinemachineFramingTransposer virtualTransposer;
    private float virtualDirection;
    private float scalePlayer;

    void Start()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        virtualTransposer = virtualCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        scalePlayer = player.transform.localScale.x;
        virtualDirection = virtualTransposer.m_TrackedObjectOffset.x;
    }
    void Update()
    {
        if (player.transform.localScale.x == scalePlayer)
        {
            virtualTransposer.m_TrackedObjectOffset.x = virtualDirection;
        }
        else if (player.transform.localScale.x == -scalePlayer)
        {
            virtualTransposer.m_TrackedObjectOffset.x = -virtualDirection;
        }
    }
}
