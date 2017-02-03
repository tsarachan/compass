/*
 * 
 * Subclass sandbox for enemy sailing vessels.
 * 
 * This class contains a variety of forms of movement. Enemy ships can mix-and-match them to get different behaviors.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : SailingShip {


	//----------Internal variables----------


	protected const string PLAYER_TAG = "Player";
	protected Transform weatherOrganizer;
	protected Transform closestCurrent = null;


	//initial variables
	protected override void Start (){
		base.Start();

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
	}


	/// <summary>
	/// Turn to follow, and then follow, the wind.
	/// </summary>
	/// <returns>A vector3 rotating between the enemy's current forward axis and the direction of the wind.</returns>
	protected Vector3 TurnWithWind(){
		return Vector3.RotateTowards(transform.forward, windScript.GetWind(), rotationSpeed ,0.0f);
	}


	/// <summary>
	/// Align with the flow of, and then follow, a current the ship is passing through.
	/// </summary>
	/// <returns>A vector3 rotating between the enemy's current forward axis and the direction of the current.</returns>
	protected Vector3 TurnWithCurrent(){
		return Vector3.RotateTowards(transform.forward,
									 current.GetComponent<Current>().GetCurrent(),
									 rotationSpeed,
									 0.0f);
	}


	/// <summary>
	/// Locate, if necessary, the closest current. Then rotate to face that current.
	/// 
	/// This works so long as the current is made up of many small objects;
	/// if the current is just a single scaled-up object it will direct the ship toward the center of the large
	/// current.
	/// </summary>
	/// <returns>A vector3 rotating between the enemy's current 
	/// forward axis and the direction toward the nearest current.</returns>
	protected Vector3 TurnToNearestCurrent(){
		if (closestCurrent == null){
			closestCurrent = FindClosestCurrent();
		}

		return Vector3.RotateTowards(transform.forward,
									 (closestCurrent.position - transform.position).normalized,
									 rotationSpeed,
									 0.0f);
	}


	/// <summary>
	/// Locate the closest current object.
	/// </summary>
	/// <returns>The closest current object.</returns>
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


	/// <summary>
	/// Detects when this ship is in a current. When it is, sets variables for other functions.
	/// </summary>
	/// <param name="other">Other.</param>
	protected override void OnTriggerStay(Collider other){
		if (other.gameObject.name.Contains(CURRENT_OBJ)){
			inCurrent = true;
			current = other.transform;
			closestCurrent = null;
		}
	}


	protected override void Turn(){
		//intentionally left blank; each enemy ship turns differently
	}
}
