/*
 * 
 * Subclass for enemies that follow the wind.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFollower : EnemyShip {


	/// <summary>
	/// Handles movement.
	/// </summary>
	protected override void Update(){
		Turn();
		base.Update();
	}


	/// <summary>
	/// Turn to follow the wind (i.e., if the wind is blowing from left to right turn toward the right).
	/// </summary>
	protected override void Turn(){
		transform.rotation = Quaternion.LookRotation(TurnWithWind());
	}
}
