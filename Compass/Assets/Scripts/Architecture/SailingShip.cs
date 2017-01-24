using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SailingShip : MonoBehaviour {

	//how fast the ship moves, before considering weather effects
	public float forwardSpeed = 1.0f;


	//how many degrees per second the ship rotates
	public float rotationSpeed = 10.0f;


	//variables for accessing the current state of the weather
	protected Wind windScript;
	protected const string WEATHER_OBJ = "Weather";
	protected const string WIND_OBJ = "Wind";


	/*
	 * 
	 * This variable helps determine how the ship is affected by wind.
	 * 
	 * It is added to the dot product of the ship's facing and the wind's direction.
	 * 
	 * If zero, the ship will be becalmed when perpendicular to the wind, and will move at normal speed
	 * when traveling with the wind.
	 * 
	 * If 1, the ship will be becalmed when facing into the wind, will move at normal speed perpendicular to it,
	 * and will move at double speed when traveling with it.
	 * 
	 */
	public float dotAdjustment = 1.0f;


	//internal variables
	protected Rigidbody rb;


	//initialize variables
	protected virtual void Start(){
		windScript = transform.root.Find(WEATHER_OBJ).GetComponent<Wind>();
		rb = GetComponent<Rigidbody>();
	}


	//move ship in the direction it is facing
	protected virtual void Update(){
		rb.MovePosition(MoveForward());
	}


	/// <summary>
	/// Determines where the ship should move, given its direction, speed, and the wind.
	/// </summary>
	/// <returns>The ship's new position.</returns>
	protected virtual Vector3 MoveForward(){
		float windEffect = Vector3.Dot(transform.forward, windScript.GetWind()) + dotAdjustment;

		return transform.position + 
			   (transform.forward * forwardSpeed * Time.deltaTime) * 
			   windEffect;
	}


	protected abstract void Turn();
}
