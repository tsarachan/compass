using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : SailingShip {

	protected const string PLAYER_TAG = "Player";
	protected Transform weatherOrganizer;
	protected Transform closestCurrent = null;


	protected override void Start (){
		base.Start();

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
	}


	protected Vector3 TurnWithWind(){
		return Vector3.RotateTowards(transform.forward, windScript.GetWind(), rotationSpeed ,0.0f);
	}


	protected Vector3 TurnWithCurrent(){
		return Vector3.RotateTowards(transform.forward,
									 current.GetComponent<Current>().GetCurrent(),
									 rotationSpeed,
									 0.0f);
	}


	protected Vector3 TurnToNearestCurrent(){
		if (closestCurrent == null){
			closestCurrent = FindClosestCurrent();
		}


		return Vector3.RotateTowards(transform.forward, closestCurrent.position, rotationSpeed, 0.0f);
	}


	protected Transform FindClosestCurrent(){
		Transform temp = transform; //default initialization for error-checking
		float distToClosest = 1000000.0f; //nonsense initialization; all currents will definitely be closer than this

		foreach (Transform current in weatherOrganizer){
			if (Vector3.Distance(transform.position, current.position) < distToClosest){
				distToClosest = Vector3.Distance(transform.position, current.position);
				temp = current;
			}
		}

		if (temp != transform){
			return temp;
		} else {
			Debug.Log("Couldn't find a current");
			return temp;
		}
	}

	protected override void OnTriggerStay(Collider other){
		Debug.Log("Encountered a trigger");
		if (other.gameObject.name.Contains(CURRENT_OBJ)){
			Debug.Log("The trigger is a current");
			inCurrent = true;
			current = other.transform;
			closestCurrent = null;
		}
	}


	protected override void Turn(){
		//intentionally left blank; each enemy ship turns differently
	}
}
