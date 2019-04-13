using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "TrackNodeToolSettings", menuName = "TrackNodeToolSettings", order = 1)]
public class TrackNodeToolSettings : ScriptableObject
{   
    public bool doDrawTrajectory = false;
    public int cycleNodeIndex = 0;

}
