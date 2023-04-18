using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

public class ItemCollector : MonoBehaviour
{
    [SerializeField] private AudioSource audioCollect;
    private int cherriesAmount = 0;
    [SerializeField] private LocalizedString localStringCherries;
    [SerializeField] private Text cherriesLegacy;

    private void OnEnable()
    {
        localStringCherries.Arguments = new object[] { cherriesAmount };
        localStringCherries.StringChanged += UpdateText;
    }

    //Como funciona el -= en estos casos??
    private void OnDisable()
    {
        localStringCherries.StringChanged -= UpdateText;
    }

    private void UpdateText(string value)
    {
        cherriesLegacy.text = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cherry"))
        {
            audioCollect.Play();
            Destroy(collision.gameObject);
            IncreaseScore();
        }
    }

    private void IncreaseScore()
    {
        cherriesAmount++;
        localStringCherries.Arguments[0] = cherriesAmount;
        localStringCherries.RefreshString();
    }
}
