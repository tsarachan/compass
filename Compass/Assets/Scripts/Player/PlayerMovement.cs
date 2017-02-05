/*
 * 
 * Handle input for player turns.
 * 
 */
using System.Collections;
using UnityEngine;

public class PlayerMovement : SailingShip {


	//player controls
	public KeyCode port = KeyCode.LeftArrow;
	public KeyCode starboard = KeyCode.RightArrow;


	//listen for input, then move in the current direction
	protected override void Update(){
		Turn();


		base.Update();
	}


	/// <summary>
	/// Turn to port or starboard if receiving the appropriate input.
	/// </summary>
	protected override void Turn(){
		if (Input.GetKey(port)){
			transform.Rotate(new Vector3(0.0f, -rotationSpeed * Time.deltaTime, 0.0f));
		} else if (Input.GetKey(starboard)){
			transform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f));
		}
	}
}
