using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AiSettings
{   
    public float maxMotorTorque = 10f;
    public float torqueAcceleration = 1f;
    public float steeringLerpTime = 0.1f;
    public int numberNodeInPrediction = 4;
    public float farNodeWeight = 0.5f;

    public float velocityDrag = 1f;

    public void SetRandomValues()
    {   
        maxMotorTorque = UnityEngine.Random.Range(30, 40);
        torqueAcceleration = UnityEngine.Random.Range(0.5f, 3f);
        steeringLerpTime = UnityEngine.Random.Range(2f, 3f);
        numberNodeInPrediction = UnityEngine.Random.Range(4, 6);
        farNodeWeight = UnityEngine.Random.Range(0.1f, 1f);
    }   
}

