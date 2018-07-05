using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AiSettings
{
    public float torqueAcceleration = 0.1f;
    public float maxMotorTorque = 10f;
    public float targetSqrSpeed = 49f;
    public float steeringLerpTime = 0.1f;
    public int numberNodeInPrediction = 4;
    public float farNodeWeight = 0.5f;

    public float velocityDrag = 1f;

    public void SetRandomValues()
    {
        targetSqrSpeed = UnityEngine.Random.Range((float)6 * 6, (float)8 * 8);
        steeringLerpTime = UnityEngine.Random.Range(2f, 3f);
        numberNodeInPrediction = UnityEngine.Random.Range(4, 6);
        farNodeWeight = UnityEngine.Random.Range(0.5f, 1.5f);
    }   
}

