using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCpidTuner : MonoBehaviour
{
    // Start is called before the first frame update
    NpcSpawner spawner;
    List<AiInfo> aiInfoList = new List<AiInfo>();

    //List<AiPid> pidList = new List<AiPid>();
    //List<AiSettings> AiSettingsList = new List<AiSettings>();
    //List<float> cumulativeError = new List<float>();
    //List<float> expectedAcceleration = new List<float>();

    public float itterationTime = 10;//s
    public float itterationTime_tracker = 0;//s


    void Start()
    {
        spawner = GetComponent<NpcSpawner>();
        foreach (GameObject item in spawner.npcList)
        {
            AiInfo nextInfo = new AiInfo(item, item.GetComponent<AiPid>(), item.GetComponent<AIDriver>().aiSettings, 0);
            nextInfo.cumulativeError = 0;
            aiInfoList.Add(nextInfo);
        }
        SetRandomPIDvalues();

    }

    // Update is called once per frame
    void Update()
    {
        itterationTime_tracker += Time.deltaTime;

        if (itterationTime_tracker >= itterationTime)
        {
            itterationTime_tracker = 0;


            //for (int i = 0; i < cumulativeError.Count; i++)
            //{ cumulativeError[i] = 0; }
        }



    }

    void SortAi()
    {

        List<AiInfo> newAiInfo = new List<AiInfo>();

        float smallest = float.PositiveInfinity;
        int position = 0;

        for (int j = 0; j < aiInfoList.Count; j++)
        {

            for (int i = j; i < aiInfoList.Count; i++)
            {
                if (aiInfoList[i].cumulativeError < smallest)
                {
                    smallest = aiInfoList[i].cumulativeError;
                    position = i;
                }
            }

            newAiInfo.Add(aiInfoList[position]);
            Swap(aiInfoList, aiInfoList, position); 
        }   

    }

    private void Swap<T>(List<T> first, List<T> Second , int position)
    {   
        T val = first[0];
        first[0] = Second[position];
        Second[position] = val;
    }   

    
    void MixTheBestRandomizeTheWorst()
    {
        float totalError = 0;
        int firstPick = 0;
        int secondPick = 0;

        for (int pick = 0; pick < 2; pick++)
        {

            for (int item = 0; item < aiInfoList.Count; item++)
            {
                totalError += aiInfoList[item].cumulativeError;

                if (item != 0)
                {
                    aiInfoList[item].cumulativeError += aiInfoList[item + 1].cumulativeError;
                }   

            }   

            do
            {
                float random = Random.Range(0, totalError);

                for (int item = 0; item < aiInfoList.Count;++item)
                {
                    if (random < aiInfoList[item].cumulativeError)
                    {
                        break;
                    }
                }

            } while (pick == 1 && firstPick == secondPick);
            
        }   

        bool crossOverParams = false;
        for (int j = 0; j < 6; j++)
        {
            if (Random.Range((int)0, (int)2) == 0)
            {crossOverParams = true;}
            else
            {crossOverParams = false;}



            if (crossOverParams)
            {   



            }   
            else
            {   




            }   


        }

    }

    public class AiInfo
    {
        public GameObject initialObj;
        public AiPid pid;
        public AiSettings setings;
        public float cumulativeError;

        public AiInfo(GameObject _initialObj, AiPid _pid, AiSettings _setings, float _cumulativeError)
        {
            initialObj = _initialObj;
            pid = _pid;
            setings = _setings;
            cumulativeError = _cumulativeError;
        }

    }


    private void SetRandomPIDvalues()
    {
        for (int i = 0; i < aiInfoList.Count; i++)
        {
            aiInfoList[i].pid.pidConstant.SetRandomConstants();
            aiInfoList[i].setings.SetRandomValues();
        }       

    }

   




}
