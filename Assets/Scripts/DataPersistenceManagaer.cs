using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManagaer : MonoBehaviour
{
    [Header("File Storage Configuration")]
    [SerializeField] private string fileName;

    private GameDataSave gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    //El private set permite que hagamos el get de forma publica desde donde sea en nuestra escena
    //pero el private set que la modifiquemos solo localmente
    //Parece que el static es tambien por el patron singleton, porque se necesita solo una instancia por escena
    public static DataPersistenceManagaer instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one Data Persistence Manager in the scene");
        }
        instance = this;
    }

    private void Start()
    {
        //Application.persistentDataPath parece ser el lugar donde vamos a guardar?
        //En mi caso el directorio es C:\Users\benja\AppData\LocalLow\Benjamin Leonardi\2DPlatformer\saveData.cdt
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameDataSave();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();
        //Load saved data with data handler
        //if there is no data, we initialize a new game

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Loading new game");
            NewGame();
        }

        //Meter el load con toda la data
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        //Pasamos data para otro scripts asi la guardan
        //Pasamos la data para el archivo que se produce, json
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        //Guardamos la data a un archivo usando el data handler
        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

}
