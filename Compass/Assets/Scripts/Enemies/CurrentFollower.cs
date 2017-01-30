using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentFollower : EnemyShip {


	protected override void Update(){
		Turn();
		base.Update();
	}


	protected override void Turn(){
		if (inCurrent){
			Debug.Log("Turning with current");
			transform.rotation = Quaternion.LookRotation(TurnWithCurrent());
		} else {
			Debug.Log("Tuning to current");
			transform.rotation = Quaternion.LookRotation(TurnToNearestCurrent());
		}
	}
}
