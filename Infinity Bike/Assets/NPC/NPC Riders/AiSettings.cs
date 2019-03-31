using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AiSettings
{
    public float targetMaximumAcceleration = 5f;
    public float targetSqrSpeed = 49f;
    public float turnSpeed = 5;
    [SerializeField]
    public TrajectoryOffset trajectoryOffset = new TrajectoryOffset();

    float trackWidth = 7;

    public void SetRandomValues()
    {   
        targetSqrSpeed = UnityEngine.Random.Range((float)36, (float)64);
        targetMaximumAcceleration = UnityEngine.Random.Range((float)1, (float)10);
        trajectoryOffset.frequency = 1f / UnityEngine.Random.Range(5, 100);
        trajectoryOffset.timeOffSet = UnityEngine.Random.Range(0, 2f * Mathf.PI)*0;
        trajectoryOffset.transverseOffset = UnityEngine.Random.Range(-trackWidth, trackWidth);
        trajectoryOffset.amplitude = UnityEngine.Random.Range(0.1f, (trackWidth - Mathf.Abs(trajectoryOffset.transverseOffset)) );
    }

    public AiSettings(AiSettings copy)
    {   
        targetMaximumAcceleration = copy.targetMaximumAcceleration;
        targetSqrSpeed = copy.targetSqrSpeed;
        turnSpeed = copy.turnSpeed;
        trajectoryOffset = copy.trajectoryOffset;
        trackWidth = copy.trackWidth;
    }

    public AiSettings()
    {
        targetMaximumAcceleration = 1;
        targetSqrSpeed =1;
        turnSpeed = 1;
    }


    [Serializable]
    public struct TrajectoryOffset
    {
        public float frequency;
        public float timeOffSet;
        public float transverseOffset;
        public float amplitude;
    }
}

