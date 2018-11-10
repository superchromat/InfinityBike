using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AiPid : MonoBehaviour {

    public float controlVariable;

    public float errorVariable;
    private float errorVariableLastFrame;

    [SerializeField]
    public PIDConstant pidConstant = new PIDConstant(1,1,1);

    [SerializeField]
    public PIDValue pidValue = new PIDValue();

    public Action UpdateErrorValue;
    private void Start()
    {
        if (pidValue == null)
        ResetValues();
    }
    
    public void RunPID()
    {
        UpdateErrorValue();

        pidValue.proportionValue = errorVariable;
        pidValue.integralValue   +=errorVariable * Time.fixedDeltaTime;
        pidValue.diffentialValue = (errorVariableLastFrame - errorVariable) / Time.fixedDeltaTime;

        controlVariable = pidConstant.kProportion * pidValue.proportionValue + pidConstant.kIntegral * pidValue.integralValue + pidConstant.kDifferential * pidValue.diffentialValue;

        errorVariableLastFrame = errorVariable;
    }

    public void ResetValues()
    {
        controlVariable = 0;
        errorVariable = 0 ;
        errorVariableLastFrame =0;
    }
    
    AiPid(
            float controlVariable,
            float kProportion,
            float kIntegral,
            float kDifferential,
            Action UpdateErrorValue
         )
        {   
            this.controlVariable = controlVariable;
            this.errorVariableLastFrame = 0;
            this.pidConstant.kProportion = kProportion;
            this.pidConstant.kIntegral = kIntegral;
            this.pidConstant.kDifferential = kDifferential;        

            errorVariable = 0;
            this.UpdateErrorValue = UpdateErrorValue;
        }   

    AiPid()
    {
        UpdateErrorValue += ()=>{ };
        this.controlVariable = 0;
        this.pidConstant.kProportion = 0;
        this.pidConstant.kIntegral = 0;
        this.pidConstant.kDifferential = 0;
        errorVariable = 0;
    }

    [Serializable]
    public class PIDConstant
    {
        public float kProportion;
        public float kIntegral;
        public float kDifferential;

        public PIDConstant(
            float kProportion,
            float kIntegral,
            float kDifferential)
        {
            this.kProportion = kProportion;
            this.kIntegral = kIntegral;
            this.kDifferential = kDifferential;
        }
    };

    [Serializable]
    public class PIDValue
    {
        public float proportionValue;
        public float integralValue;
        public float diffentialValue;
    };


}
