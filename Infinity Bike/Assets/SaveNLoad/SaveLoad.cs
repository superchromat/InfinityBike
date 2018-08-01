using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveLoad
{
    public string fileName = "track_1";
    public string extension = ".track";
    public string dataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    
    public class SaveLoadUtilities<T>
    {
        public static void Save(T serializableClass, string fileName, string dataPath)
        {

            if (dataPath == null)
            {dataPath = Application.dataPath;}

            string destination = dataPath + "\\" + fileName;
            
            FileStream file;

            if (File.Exists(destination)) file = File.OpenWrite(destination)/*+".dat"*/;
            else file = File.Create(destination);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(file, serializableClass);
            file.Close();
            Debug.Log("Saved to : " + destination);
        }   

        public static void Load(out T serializableClass, string fileName, string dataPath)
        {
            if (dataPath == null)
            {dataPath = Application.dataPath;}
            string destination = dataPath + "/"+ fileName;
            FileStream file;

            if (File.Exists(destination))
            {
                file = File.OpenRead(destination);
                BinaryFormatter bf = new BinaryFormatter();
                serializableClass = (T)bf.Deserialize(file);
                file.Close();
            }
            else
            {
                serializableClass = (T)Activator.CreateInstance(typeof(T), new object[] { });
            }
            Debug.Log("Load from to : " + destination);


        }

    }
}