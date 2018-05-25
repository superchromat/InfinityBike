using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMovement : MonoBehaviour {

	Rigidbody playerRB;
	public float speed = 1f;
	public float gravity = 1f;
	public float rawRotation = 0;
	public float angleChangeRate = 1f;
	public float drag = 0.1f;

	public RotatioRange rotationRange = new RotatioRange();
	public ArduinoThread storageValue;
	private float speedThreshold = 0.5f;
	public bool isLanded = false;
    public Transform handleBar;
    public float handleBarMultiplicator = 5f;
	// Use this for initialization
	void Start () 
	{
		playerRB = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void Update () {

		rawRotation = storageValue.values.rotation;

		float angle = (rawRotation / ((rotationRange.maxRawRotation - rotationRange.minRawRotation)) - 0.5f)*angleChangeRate;
       
        handleBar.localRotation =  Quaternion.Euler(0, angle * handleBarMultiplicator+90, 90);
		Vector3 moveDir = Quaternion.Euler (0, angle, 0) *transform.forward * (float)storageValue.values.speed * speed / 1024f;

		
		if (moveDir.sqrMagnitude > speedThreshold) {
			
			playerRB.velocity = moveDir;
            transform.forward = new Vector3 (playerRB.velocity.x, playerRB.velocity.y, playerRB.velocity.z);

		} else {
			playerRB.velocity *= drag;
		}

		//if(!isLanded)
		//playerRB.velocity += new Vector3 (0, -gravity * Time.deltaTime, 0);
		

	}	

	[Serializable]
	public struct RotatioRange
	{
		public float maxRawRotation;
		public float minRawRotation;

		public RotatioRange(float maxRawRotation,float minRawRotation)
		{
			this.maxRawRotation = maxRawRotation;
			this.minRawRotation = minRawRotation;
		}
	}

	void OnCollisionStay(Collision col)
	{
		isLanded = false;
		if (col.contacts.Length > 0) 
		{
			Vector3 avgContact = Vector3.zero;
			for (int i = 0; i < col.contacts.Length; i++) {
				Debug.DrawRay (transform.position, (col.contacts [i].point - transform.position));
				avgContact += (col.contacts [i].point - transform.position);
			} 


			avgContact = avgContact.normalized;

			Debug.DrawRay (transform.position, avgContact, Color.red);

			if (Vector3.Dot (avgContact, Vector3.down) > 0.8f) {
				isLanded = true;
			}
		}

	}
	void OnCollisionExit(Collision col)
	{
		isLanded = false;
	}


}
