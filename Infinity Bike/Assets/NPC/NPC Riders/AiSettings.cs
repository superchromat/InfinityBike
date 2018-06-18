using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
[Serializable]

public class AiSettings
{   
    
    public float maxMotorTorque = 10f;
    public float torqueAcceleration = 1f;
    public float steeringLerpTime = 0.1f;
    public float velocityDrag = 1f;
}

public static class SaveLoad
{

    public static List<AiSettings> savedGames = new List<AiSettings>();

    public static void Save(AiSettings aiSettings)
    {   
        savedGames.Add(aiSettings);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
        Debug.Log("saved at " + Application.persistentDataPath + "/savedGames.gd");
    }

    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            SaveLoad.savedGames = (List<AiSettings>)bf.Deserialize(file);
            file.Close();
        }
    }
}