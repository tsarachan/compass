using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFollower : EnemyShip {

	protected override void Update(){
		Turn();
		base.Update();
	}


	protected override void Turn(){
		transform.rotation = Quaternion.LookRotation(TurnWithWind());
	}
}
