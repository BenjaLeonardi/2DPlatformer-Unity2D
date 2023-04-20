using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    //Constructor
    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameDataSave Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameDataSave loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                //Load serialized data from Json
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Deserialize from Json into C# Object
                loadedData = JsonUtility.FromJson<GameDataSave>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public void Save(GameDataSave dataSave)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            //Create directory path in case it doesnt exist in our pc
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //Serialize gamedata obj into a Json file
            string dataToStore = JsonUtility.ToJson(dataSave, true);

            //Write file to datasystem
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }
}
