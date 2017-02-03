/*
 * 
 * Subclass for enemies that move in a circle. These enemies will follow currents to the left until they are close
 * to leaving the screen, and then follow the wind back to the right.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circler : EnemyShip {


	//----------Internal variables----------

	public float edgeBuffer = 10.0f; //how far from the left and right edges of the screen the ship will start to turn
	private bool followingWind = true; //true if the ship is following the wind, false if it's following the currents


	//delegate that stores the function the ship will use to move in a way that keeps it on the screen.
	private delegate Vector3 MovementFunction();
	private MovementFunction movementFunction;


	/// <summary>
	/// Handles movement. First determine whether, based on screen position, it's necessary to move with the
	/// wind or the current in order to stay on the screen. Then assign a movement function accordingly, use it
	/// to turn, and move forward.
	/// </summary>
	protected override void Update(){
		followingWind = DecideWhatToFollow();

		movementFunction = ChooseDirection;

		Turn();

		base.Update();
	}


	/// <summary>
	/// Determine whether the ship should follow the wind to the right or currents to the left, with the 
	/// goal of staying on the screen.
	/// </summary>
	/// <returns><c>true</c> if the ship should follow the wind, <c>false</c> if it should
	/// follow the currents.</returns>
	private bool DecideWhatToFollow(){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		bool temp = followingWind;

		if (screenPos.x - edgeBuffer <= 0.0f){
			temp = true;
		} else if (screenPos.x + edgeBuffer >= Screen.width){
			temp = false;
		}

		return temp;
	}


	/// <summary>
	/// Turn the ship using whichever movement function is appropriate.
	/// </summary>
	protected override void Turn(){
		transform.rotation = Quaternion.LookRotation(movementFunction());
	}


	/// <summary>
	/// Assign a movement function to the delegate that's used for turning.
	/// </summary>
	/// <returns>The direction returned by the appropriate movement function.</returns>
	private Vector3 ChooseDirection(){
		if (followingWind){
			return TurnWithWind();
		} else {
			return FindOrFollowCurrent();
		}
	}


	/// <summary>
	/// If the ship needs to follow the currents, assign the proper movement function--either one to find
	/// a current, or one to follow it.
	/// </summary>
	/// <returns>The direction returned by the appropriate movement function.</returns>
	private Vector3 FindOrFollowCurrent(){
		if (inCurrent){
			return TurnWithCurrent();
		} else {
			return TurnToNearestCurrent();
		}
	}
}
