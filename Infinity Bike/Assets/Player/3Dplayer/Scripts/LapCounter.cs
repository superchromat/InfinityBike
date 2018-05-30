using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCounter : MonoBehaviour {

	private List<Collider> objectCrossingList = new List<Collider>();


	void Update()
	{
		List<Collider> tempObjectCrossingList = new List<Collider>(objectCrossingList);
		for (int count = 0; count < tempObjectCrossingList.Count; count++) 
		{
			PlayerPoints playerPoints = tempObjectCrossingList [count].gameObject.GetComponent<PlayerPoints> ();
			if (playerPoints != null) 
			{
				playerPoints.IncrementPoints ();
			}
		}

		for (int count = 0; count < tempObjectCrossingList.Count; count++)
		{objectCrossingList.Remove (tempObjectCrossingList [count]);}
	}



	void OnTriggerEnter(Collider other)
	{
		if (objectCrossingList.IndexOf (other) < 0) {
			objectCrossingList.Add (other);
		}
	}

}	
