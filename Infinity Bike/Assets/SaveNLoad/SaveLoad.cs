using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoad<T>
{   
    public static void Save(T serializableClass ,string fileName)
    {
        string destination = Application.persistentDataPath + "/"+ fileName /*+".dat"*/;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination)/*+".dat"*/;
        else file = File.Create(destination);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, serializableClass);
        file.Close();
        Debug.Log("Saved to : " + destination);

    }

    public static void Load(out T serializableClass, string fileName)
    {   
        string destination = Application.persistentDataPath + "/"+ fileName;
        FileStream file;
        Debug.Log(destination);

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
            BinaryFormatter bf = new BinaryFormatter();
            serializableClass = (T)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            serializableClass = (T)Activator.CreateInstance(typeof(T), new object[] { }); ;
        }
    }      



}   
