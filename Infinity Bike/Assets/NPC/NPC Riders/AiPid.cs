using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AiPid : MonoBehaviour {

    public float controlVariable;

    public float errorVariable;
    private float errorVariableLastFrame;

    public float proportionValue;
    public float integralValue;
    public float diffentialValue;

    public float kProportion;
    public float kIntegral;
    public float kDifferential;

    public Action UpdateErrorValue;
    public Action ReactToControlVariable;
    private void Start()
    {
        ResetValues();
    }

    private void FixedUpdate()
    {
        UpdateErrorValue();

        proportionValue = errorVariable;
        integralValue += errorVariable * Time.fixedDeltaTime;
        diffentialValue = (errorVariableLastFrame - errorVariable) / Time.fixedDeltaTime;

        controlVariable = kProportion * proportionValue + kIntegral * integralValue + kDifferential * diffentialValue;
        
        ReactToControlVariable();
        errorVariableLastFrame = errorVariable;
    }
    public void ResetValues()
    {
        controlVariable = 0; ;
        errorVariable = 0 ;
        errorVariableLastFrame =0;
        proportionValue = 0;
        integralValue = 0;
        diffentialValue = 0;
    }



    AiPid(
            float controlVariable,
            float kProportion,
            float kIntegral,
            float kDifferential,
            Action UpdateErrorValue,
            Action ReactToControlVariable
         )
    {   
        this.controlVariable = controlVariable;
        this.errorVariableLastFrame = 0;
        this.kProportion = kProportion;
        this.kIntegral = kIntegral;
        this.kDifferential = kDifferential;
        integralValue = 0;
        errorVariable = 0;
        this.UpdateErrorValue = UpdateErrorValue;
        this.ReactToControlVariable = ReactToControlVariable;
    }   

    AiPid()
    {
        ReactToControlVariable += () => { };
        UpdateErrorValue += ()=>{ };
        this.controlVariable = 0;
        this.kProportion = 0;
        this.kIntegral = 0;
        this.kDifferential = 0;
        integralValue = 0;
        errorVariable = 0;
    }   
}   
