using System.Collections;
using UnityEngine;

public class Current : MonoBehaviour {


	//the direction of this current' object's flow, and the speed with which it flows
	//these are public so that they can be tuned in the inspector; scripts should not access them directly
	public Vector3 direction = -Vector3.right;
	public float speed = 1.0f;


	/// <summary>
	/// Get the current's flow, in a form ships can use to affect their movement.
	/// </summary>
	/// <returns>A Vector3 reflecting the direction and speed of the curretn.</returns>
	public Vector3 GetCurrent(){
		return direction.normalized * speed;
	}
}
