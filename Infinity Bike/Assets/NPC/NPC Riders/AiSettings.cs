using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AiSettings
{
    public float targetSqrSpeed = 49f;
    public float turnSpeed = 5;
    [SerializeField]
    public TrajectoryOffset trajectoryOffset = new TrajectoryOffset();

    float trackWidth = 7;

    public void SetRandomValues()
    {   
        targetSqrSpeed = UnityEngine.Random.Range((float)36, (float)49);
        trajectoryOffset.frequency = 1f / UnityEngine.Random.Range(1, 100);
        trajectoryOffset.timeOffSet = UnityEngine.Random.Range(0, 2f * Mathf.PI)*0;
        trajectoryOffset.transverseOffset = UnityEngine.Random.Range(-trackWidth, trackWidth);
        trajectoryOffset.amplitude = UnityEngine.Random.Range(0.1f, (trackWidth - Mathf.Abs(trajectoryOffset.transverseOffset)) );
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

