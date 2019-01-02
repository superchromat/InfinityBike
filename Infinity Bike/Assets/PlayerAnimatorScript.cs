using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour {
    private Animator playerAnimator;

    //[SerializeField]

    //public float steerAngle;
    



    // Use this for initialization
    void Start () {
        playerAnimator = GetComponent<Animator>();

       

    }
	
	
	public void UpdateSteeringAngle (float angle) {
        float normalizedAngle = (angle + 45)/90;
        //Debug.Log(normalizedAngle);
        playerAnimator.Play("Steering", 1, normalizedAngle);
	}
    public void UpdateCyclingSpeed(float torque)
    {
        playerAnimator.SetFloat("cyclingSpeed", torque);
    }
   

}
