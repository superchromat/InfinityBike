using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour {
    private Animator playerAnimator;
    private float wheelRotationSpeed = 1;
    public float cyclingSpeedFactor  = 1f;
    public float wheelRotationFactor = 10; 
    public Transform frontWheel;
    public Transform backWheel; 

    //[SerializeField]

    //public float steerAngle;
    



    // Use this for initialization
    void Start () {
        playerAnimator = GetComponent<Animator>();

       

    }
    private void FixedUpdate()
    {
        //UpdateWheel rotation
<<<<<<< HEAD:Infinity Bike/Assets/Player/PlayerAnimatorScript.cs
        frontWheel.Rotate(0,wheelRotationSpeed*wheelRotationFactor * Time.deltaTime, 0);
        //The front wheel rotates in y because of mixup with Blender. The front
        //wheel is bound to a bone...
        backWheel.Rotate(wheelRotationSpeed * wheelRotationFactor*Time.deltaTime, 0, 0);
    }


=======
        frontWheel.Rotate(wheelRotationSpeed*wheelRotationFactor * Time.deltaTime, 0, 0);
        backWheel.Rotate(wheelRotationSpeed * wheelRotationFactor*Time.deltaTime, 0, 0);
    }


>>>>>>> 346887c691f365e571c7b7d89e62dbdba5decb98:Infinity Bike/Assets/PlayerAnimatorScript.cs
    public void UpdateSteeringAngle (float angle) {
        float normalizedAngle = (-angle + 45)/90;
        //Debug.Log(normalizedAngle);
        playerAnimator.Play("Steering", 1, normalizedAngle);
	}
    public void UpdateCyclingSpeed(float torque)
    {
        playerAnimator.SetFloat("cyclingSpeed", torque* cyclingSpeedFactor );
    }
    public void UpdateWheelRotationSpeed()
    {



    }


}
