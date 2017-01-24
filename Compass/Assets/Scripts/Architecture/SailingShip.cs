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


	//internal variables
	protected Rigidbody rb;


	protected virtual void Start(){
		windScript = transform.root.Find(WEATHER_OBJ).GetComponent<Wind>();
		rb = GetComponent<Rigidbody>();
	}

	protected virtual void Update(){
		rb.MovePosition(MoveForward());
	}


	protected virtual Vector3 MoveForward(){
		return transform.position + (transform.forward * forwardSpeed * Time.deltaTime);
	}


	protected abstract void Turn();
}
