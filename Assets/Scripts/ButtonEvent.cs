using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject Player;
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pressed");
        Player.GetComponent<PlayerMovemente>().PlayerJump();
    }

    // Button is released
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Released");

    }
}
