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
			transform.rotation = Quaternion.LookRotation(TurnWithCurrent());
		} else {
			transform.rotation = Quaternion.LookRotation(TurnToNearestCurrent());
		}
	}
}
