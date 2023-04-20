using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameDataSave
{
    public int cherriesAmount;

    //los valores que definamos aca van a ser los default
    //cuando el juego cargue sin un save state
    public GameDataSave()
    {
        this.cherriesAmount = 0;
    }
}
