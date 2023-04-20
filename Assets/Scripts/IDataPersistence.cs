using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameDataSave dataSave);
    void SaveData(ref GameDataSave dataSave);
}
