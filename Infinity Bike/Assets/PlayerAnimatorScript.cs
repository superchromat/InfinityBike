using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour {
    private Animator playerAnimator;

   //[SerializeField]
    private float steerAngle; 
    public float SteerAngle
    {
        get
        {
            return steerAngle;
        }
        set
        {
            steerAngle = value;
            Debug.Log("Set");
            UpdateSteerAngle();
        }
    }




    // Use this for initialization
    void Start () {
        playerAnimator = GetComponent<Animator>();

       

    }
	
	// Update is called once per frame
	void UpdateSteerAngle () {
        playerAnimator.Play("Steering", 1, steerAngle);
	}
}
