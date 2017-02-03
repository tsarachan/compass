/*
 * 
 * Subclass for enemies who follow currents from one side of the scren to the other.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFollower : EnemyShip {


	/// <summary>
	/// Handles movement.
	/// </summary>
	protected override void Update(){
		Turn();
		base.Update();
	}


	/// <summary>
	/// If in a current, turn to face the direction of its flow. If not, turn toward the nearest current object.
	/// </summary>
	protected override void Turn(){
		if (inCurrent){
			transform.rotation = Quaternion.LookRotation(TurnWithCurrent());
		} else {
			transform.rotation = Quaternion.LookRotation(TurnToNearestCurrent());
		}
	}
}
