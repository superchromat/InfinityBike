using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[CreateAssetMenu(fileName = "TrackNode", menuName = "TrackNode", order = 1)]
public class TrackNode : ScriptableObject 
{
    public TrackNodeValues nodeValues;

    public void DeleteNode(int index)
    {
        Debug.Log(index);
        nodeValues.nodeList.RemoveAt(index);
    }

    public void InsertNode(Vector3 toadd, int indexAhead)
    {
        if (indexAhead < 0)
        {
            indexAhead += nodeValues.nodeList.Count;
        }
        else if (indexAhead > nodeValues.nodeList.Count)
        {
            indexAhead -= nodeValues.nodeList.Count;
        }

        nodeValues.nodeList.Insert(indexAhead, toadd);
    }

    public void AddNode (Vector3 toadd)
	{

        RaycastHit hit;
        if (Physics.Raycast(toadd, Vector3.down, out hit, 100f))
        { nodeValues.nodeList.Add(hit.point + Vector3.up * 1.5f); }
        else if(Physics.Raycast(toadd, Vector3.up, out hit, 100f))
        { nodeValues.nodeList.Add(hit.point + Vector3.up * 1.5f); }


    }	
	
	public void SetNode (Vector3 toadd, int index)
	{	
		if(nodeValues.nodeList.Count != 0)
		{
            nodeValues.nodeList[index] = toadd;
		}	
	}


	public Vector3 GetNode (int index)
	{

		if (GetNodeCount () == 0) 
		{
            return Vector3.zero;
        }	

		while (index >= nodeValues.nodeList.Count) 
		{index -= nodeValues.nodeList.Count;}	

		while (index < 0) 
		{index += nodeValues.nodeList.Count;}

        return nodeValues.nodeList[index];
	}

	public int GetNodeCount()
	{
		return nodeValues.nodeList.Count;
	}
    
}

[Serializable]
public class TrackNodeValues
{   
    public List<Vector3> nodeList = new List<Vector3>();

    [Serializable]
    private class Vector3Serialisable
    {
        public List<float> x;
        public List<float> y;
        public List<float> z;

        public Vector3Serialisable(List<Vector3> vec)
        {
            x = new List<float>();
            y = new List<float>();
            z = new List<float>();

            foreach (Vector3 item in vec)
            {
                x.Add(item.x);
                y.Add(item.y);
                z.Add(item.z);
            }

        }
        public Vector3Serialisable()
        {
            x = new List<float>();
            y = new List<float>();
            z = new List<float>();
        }

        public void SetValuesToNodeList(out List<Vector3> nodeList )
        {   
            nodeList = new List<Vector3>();
            for (int i = 0; i < x.Count; i++)
            {nodeList.Add(new Vector3(x[i],y[i], z[i]));}
        }   

    }

    public void Save()
    {
        Vector3Serialisable nodeListSerialisable = new Vector3Serialisable(nodeList);
        SaveLoad<Vector3Serialisable>.Save(nodeListSerialisable);


        /*
        Vector3Serialisable nodeListSerialisable = new Vector3Serialisable(nodeList);

        string destination = Application.persistentDataPath + "/save.dat";
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);
        BinaryFormatter bf = new BinaryFormatter();

        bf.Serialize(file, nodeListSerialisable);
        file.Close();
        */
    }

    public void LoadFile()
    {
        
        Vector3Serialisable nodeListSerialisable = new Vector3Serialisable();
        SaveLoad<Vector3Serialisable>.Load(out nodeListSerialisable);
        nodeListSerialisable.SetValuesToNodeList(out nodeList);
        
        
        /*
          
        Vector3Serialisable data = new Vector3Serialisable();

        string destination = Application.persistentDataPath + "/save.dat";

        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        data = (Vector3Serialisable)bf.Deserialize(file);

        nodeList.Clear();
        data.SetValuesToNodeList(out nodeList);

        file.Close();
        */

    }
}   