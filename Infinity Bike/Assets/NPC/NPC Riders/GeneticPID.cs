using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPID : MonoBehaviour {

    public float wheelyErrorFrequency = 1f;
    public float accelerationTime = 3;
    public float lastAverageError = 0;
    AiSettings AiSettingsList = new AiSettings();


    NPCspawner npcSpawner;
    public float timeSinceLastRestart = 0f;
    public float recordTime = 10f;
    public int numberOfRandom = 10;

    float allTimeBestScore = float.MaxValue;
    AiPid.PIDConstant allTimeBestAI;
    public List<AI> AI_PID_TunningParameters = new List<AI>();
    private List<AI> AI_PID_TunningParameters_Sorted = new List<AI>();
    [System.Serializable]
    public class AI
    {

        public AIDriver aiDriver;
        public AiPid aiPid;
        public float cumulativeError;
        public float timeAlive;

        public AI(AiPid aiPid, AIDriver aiDriver, float cumulativeError)
        {
            timeAlive = 0;
            this.aiDriver = aiDriver;
            this.aiPid = aiPid;
            this.cumulativeError = cumulativeError;

        }   

    }
    
    private void Start()
    {   
        npcSpawner = GetComponent<NPCspawner>();
        
        AiSettingsList.SetRandomValues();
        AI_PID_TunningParameters.Clear();
        int count = 0;
        foreach (GameObject item in npcSpawner.npcList)
        {   
            item.transform.gameObject.SetActive(true);
            item.transform.gameObject.GetComponent<AIDriver>().IdleMode = false;

            AiPid aiPid =  item.GetComponent<AiPid>();
            AIDriver aiDriver = item.transform.gameObject.GetComponent<AIDriver>();

            AI_PID_TunningParameters.Add(new AI(aiPid, aiDriver, 0));

            aiDriver.IdleMode = false;
            setPIDConstantRandom(AI_PID_TunningParameters[count].aiPid.pidConstant);
            

            AI holder = AI_PID_TunningParameters[count];
            //rsp.AddToRespawnAction(() => { holder.cumulativeError += 1000; });
            
            count++;
        }   
    }

    void setPIDConstantRandom(AiPid.PIDConstant aiPid)
    {
        aiPid.kProportion = Random.Range(0f, 10f);
        aiPid.kIntegral = Random.Range(0f, 10f);
        aiPid.kDifferential = Random.Range(0f, 10f);
    }

    bool firstFrame = true;
    private void Update()
    {   

        if (firstFrame)
        {
            foreach (AI item in AI_PID_TunningParameters)
            {
                item.cumulativeError = 0;
            }
            firstFrame = false;
        }

        bool isDone = true;
        timeSinceLastRestart += Time.deltaTime;
        for (int i = 0;i < AI_PID_TunningParameters.Count;i++)
        {

            if (AI_PID_TunningParameters[i].timeAlive >= recordTime || timeSinceLastRestart > recordTime)
            { continue; }

            isDone = false;

            if (AI_PID_TunningParameters[i].aiDriver.IdleMode == true )
            { continue; }
            

            float currentError = 0;
            AI_PID_TunningParameters[i].timeAlive += Time.deltaTime;

            if (AI_PID_TunningParameters[i].timeAlive > accelerationTime)
            {
                currentError += Mathf.Abs(AI_PID_TunningParameters[i].aiPid.errorVariable);
                Vector3 normal = AI_PID_TunningParameters[i].aiDriver.WheelNormal;
                float normalAllignement = Vector3.Dot(normal.normalized, AI_PID_TunningParameters[i].aiDriver.transform.up);
                currentError += (1 - normalAllignement) * wheelyErrorFrequency;
            }
            AI_PID_TunningParameters[i].cumulativeError += currentError*Time.deltaTime;
            
            if (AI_PID_TunningParameters[i].timeAlive > recordTime)
            {AI_PID_TunningParameters[i].aiDriver.IdleMode = true;}
        }   


        if (isDone)
        {
            firstFrame = true;

            ClampError();
            SortAI();
            
            AssignPID(GenerateNewPIDValues());
            timeSinceLastRestart = 0;

            for (int i = 0; i < AI_PID_TunningParameters.Count; i++)
            {
                AI_PID_TunningParameters[i].timeAlive = 0;
                Respawn rsp = AI_PID_TunningParameters[i].aiPid.transform.gameObject.GetComponent<Respawn>();
                rsp.respawnNode = 0;
                AI_PID_TunningParameters[i].aiPid.controlVariable  = 0;
                AI_PID_TunningParameters[i].aiPid.errorVariable = 0;
                AI_PID_TunningParameters[i].aiPid.transform.gameObject.GetComponent<AIDriver>().IdleMode = false;

                AI_PID_TunningParameters[i].cumulativeError = 0;

                rsp.CallRespawnAction();

            }

        }

    }



    void ClampError()
    {
        lastAverageError = 0;
        foreach (AI item in AI_PID_TunningParameters)
        {
            item.cumulativeError = item.cumulativeError / item.timeAlive;
            //item.cumulativeError = (float)System.Math.Tanh((double)item.cumulativeError);
            lastAverageError += item.cumulativeError;
        }
        lastAverageError /= (float)AI_PID_TunningParameters.Count;

    }

    void SortAI()
    {   
        AI[] aiArr = AI_PID_TunningParameters.ToArray();
        System.Array.Sort<AI>(aiArr, (x,y)=> x.cumulativeError.CompareTo(y.cumulativeError));

        AI_PID_TunningParameters_Sorted.Clear();
        AI_PID_TunningParameters_Sorted = new List<AI>(aiArr);
        Debug.Log("BestScore : " + aiArr[0].cumulativeError +"\n" + "WorstScore : " + aiArr[aiArr.Length-1].cumulativeError);

        if (allTimeBestScore > AI_PID_TunningParameters_Sorted[0].cumulativeError)
        {
            allTimeBestScore = AI_PID_TunningParameters_Sorted[0].cumulativeError;

            allTimeBestAI = new AiPid.PIDConstant(AI_PID_TunningParameters_Sorted[0].aiPid.pidConstant.kProportion, AI_PID_TunningParameters_Sorted[0].aiPid.pidConstant.kIntegral, AI_PID_TunningParameters_Sorted[0].aiPid.pidConstant.kDifferential);
        }
        Debug.Log("Alltime best score : " + allTimeBestScore);
        Debug.Log( " P : " + allTimeBestAI.kProportion + " I : "+ allTimeBestAI.kIntegral + " D : " + allTimeBestAI.kDifferential);
        
    }   

    List<AiPid.PIDConstant> GenerateNewPIDValues()
    {
        List<AiPid.PIDConstant> newList = new List<AiPid.PIDConstant>(AI_PID_TunningParameters_Sorted.Count*2);

        for (int i = 0; i < AI_PID_TunningParameters_Sorted.Count-1; i++)
        {
            AI_PID_TunningParameters_Sorted[i].cumulativeError = Mathf.Abs(AI_PID_TunningParameters_Sorted[i].cumulativeError);
            if (i != 0)
            {

                if (AI_PID_TunningParameters_Sorted[i].cumulativeError != 0)
                    AI_PID_TunningParameters_Sorted[i].cumulativeError = AI_PID_TunningParameters_Sorted[i - 1].cumulativeError + 1 / AI_PID_TunningParameters_Sorted[i].cumulativeError;
                else
                    AI_PID_TunningParameters_Sorted[i].cumulativeError = AI_PID_TunningParameters_Sorted[i - 1].cumulativeError+1;
            }   
            else
            {
                if (AI_PID_TunningParameters_Sorted[i].cumulativeError != 0)
                    AI_PID_TunningParameters_Sorted[i].cumulativeError = 1 / AI_PID_TunningParameters_Sorted[i].cumulativeError;
                else
                    AI_PID_TunningParameters_Sorted[i].cumulativeError = 0.0001f;
            }   
        }   

        for(int j = 0; j < AI_PID_TunningParameters_Sorted.Count; j ++)
        {
            float pick1 = Random.Range(0, AI_PID_TunningParameters_Sorted[AI_PID_TunningParameters_Sorted.Count - 1].cumulativeError);

            for (int i = 0; i < AI_PID_TunningParameters_Sorted.Count; i++)
            {   
                if (pick1 < AI_PID_TunningParameters_Sorted[i].cumulativeError)
                {   
                    newList.Add(AI_PID_TunningParameters_Sorted[i].aiPid.pidConstant);
                    break;
                }   
            }
            bool done = false;
            do
            {
                float pick2 = Random.Range(0, AI_PID_TunningParameters_Sorted[AI_PID_TunningParameters_Sorted.Count - 1].cumulativeError);
                for (int i = 0; i < AI_PID_TunningParameters_Sorted.Count; i++)
                {
                    if (pick2 < AI_PID_TunningParameters_Sorted[i].cumulativeError && newList[newList.Count - 1] != AI_PID_TunningParameters_Sorted[i].aiPid.pidConstant)
                    {
                        done = true;
                        newList.Add(AI_PID_TunningParameters_Sorted[i].aiPid.pidConstant);
                        break;
                    }
                }
            } while (!done);


        }
        
        List<AiPid.PIDConstant> finalNewList = new List<AiPid.PIDConstant>(AI_PID_TunningParameters_Sorted.Count );
        finalNewList.Add(new AiPid.PIDConstant( allTimeBestAI.kProportion, allTimeBestAI.kIntegral, allTimeBestAI.kDifferential));
        int count = 0;
        {
            int j = 0;
            while (finalNewList.Count < AI_PID_TunningParameters_Sorted.Count)
            {
                finalNewList.Add(new AiPid.PIDConstant(0, 0, 0));
                count = finalNewList.Count;

                int randP = Random.Range(0, 2);
                int randI = Random.Range(0, 2);
                int randD = Random.Range(0, 2);

                if (randP == 0)
                    finalNewList[count - 1].kProportion = newList[j].kProportion;
                else if (randP == 1)
                    finalNewList[count - 1].kProportion = newList[j + 1].kProportion;


                if (randI == 0)
                    finalNewList[count - 1].kIntegral = newList[j].kIntegral;
                else if (randI == 1)
                    finalNewList[count - 1].kIntegral = newList[j + 1].kIntegral;


                if (randD == 0)
                    finalNewList[count - 1].kDifferential = newList[j].kDifferential;
                else if (randD == 1)
                    finalNewList[count - 1].kDifferential = newList[j + 1].kDifferential;


                for (int k = 0; k < 3; k++)
                {
                    int rand = Random.Range(0, 50);
                    if (rand == 0)
                    {

                        float val = Random.Range(0, 10);
                        if (j == 0)
                        { finalNewList[count - 1].kProportion = val; }
                        else if (j == 1)
                        { finalNewList[count - 1].kIntegral = val; }
                        else
                        { finalNewList[count - 1].kDifferential = val; }

                    }
                }

                j += 2;
            }
        }

        for (int i = 0; i < numberOfRandom; i++)
        {setPIDConstantRandom(finalNewList[finalNewList.Count - i - 1]);}

        return finalNewList;
    }

    void AssignPID(List<AiPid.PIDConstant> newList)
    {

        for (int i = 0; i < AI_PID_TunningParameters.Count; i++)
        {
            AI_PID_TunningParameters[i].aiPid.pidConstant = newList[i];
        }   

    }
























}
