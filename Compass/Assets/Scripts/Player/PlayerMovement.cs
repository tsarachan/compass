using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : SailingShip {


	//player controls
	public KeyCode port = KeyCode.LeftArrow;
	public KeyCode starboard = KeyCode.RightArrow;


	protected override void Update(){
		Turn();
		rb.MovePosition(MoveForward());
	}

	protected override void Turn(){
		if (Input.GetKey(port)){
			transform.Rotate(new Vector3(0.0f, -rotationSpeed * Time.deltaTime, 0.0f));
		} else if (Input.GetKey(starboard)){
			transform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f));
		}
	}
}
