using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AiSettings
{
    public float maxMotorTorque = 10f;
    public float targetSqrSpeed = 49f;

    public void SetRandomValues()
    {   
        targetSqrSpeed = UnityEngine.Random.Range((float)36, (float)64);
    }      
}

