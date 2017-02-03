using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circler : EnemyShip {

	public float edgeBuffer = 10.0f;
	private bool followingWind = true;

	private delegate Vector3 MovementFunction();
	private MovementFunction movementFunction;


	protected override void Update(){
		followingWind = DecideWhatToFollow();

		movementFunction = ChooseDirection;

		Turn();
		base.Update();
	}


	private bool DecideWhatToFollow(){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		bool temp = followingWind;

//		Debug.Log("screenPos.x == " + screenPos.x + ", Screen.width == " + Screen.width);

		if (screenPos.x - edgeBuffer <= 0.0f){
			temp = true;
		} else if (screenPos.x + edgeBuffer >= Screen.width){
//			Debug.Log("Leaving screen");
			temp = false;
		}

		return temp;
	}


	protected override void Turn(){
//		Debug.Log(movementFunction());
		transform.rotation = Quaternion.LookRotation(movementFunction());
	}


	private Vector3 ChooseDirection(){
		if (followingWind){
			return TurnWithWind();
		} else {
			return FindOrFollowCurrent();
		}
	}

	private Vector3 FindOrFollowCurrent(){
		if (inCurrent){
			return TurnWithCurrent();
		} else {
			return TurnToNearestCurrent();
		}
	}
}
