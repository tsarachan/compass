using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {


	//the direction and speed of the wind
	//these are public so that they can be set in the editor; they should never be accessed by another script
	public Vector3 direction;
	public float speed = 2.0f;


	/// <summary>
	/// Get the wind in a form suitable for applying force to ships.
	/// </summary>
	/// <returns>The wind, as a Vector3.</returns>
	public Vector3 GetWind(){
		return direction.normalized * speed;
	}
}
