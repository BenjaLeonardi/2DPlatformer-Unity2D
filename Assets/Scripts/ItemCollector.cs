using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;

public class ItemCollector : MonoBehaviour, IDataPersistence
{
    [SerializeField] private AudioSource audioCollect;
    private int cherriesAmount;
    [SerializeField] private LocalizedString localStringCherries;
    [SerializeField] private Text cherriesLegacy;

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

    //Estos ultimos dos metodos se ejecutan al instanciarse el DataPersistenceManager en la escena, de forma tal que podemo reutilizar
    //su codigo para hacer mas cosas de las que hacemos aca
    public void LoadData(GameDataSave dataSave)
    {
        this.cherriesAmount = dataSave.cherriesAmount;

        //Previamente esto estaba en OnEnable() pero no recargaba el savegame al empezar el juego, por lo tanto lo incorpore aca. Ver si se puede optimizar.
        localStringCherries.Arguments = new object[] { cherriesAmount };
        localStringCherries.StringChanged += UpdateText;
    }

    public void SaveData(ref GameDataSave dataSave)
    {
        dataSave.cherriesAmount = this.cherriesAmount;
    }
}
