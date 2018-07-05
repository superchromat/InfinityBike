using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class SaveLoad<T>
{   
    public static void Save(T serializableClass ,string fileName)
    {

        string destination = Application.persistentDataPath + "/"+ fileName +".dat";
        Debug.Log(destination);
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, serializableClass);
        file.Close();
    }

    public static void Load(out T serializableClass, string fileName)
    {   
        string destination = Application.persistentDataPath + "/"+ fileName +".dat";
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
