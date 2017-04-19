using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeTask : Task {

	private AICombatant myShip;
	private Transform fleeFrom;
	private float fleeDist;


	public FleeTask(AICombatant myShip, Transform fleeFrom, float fleeDist){
		this.myShip = myShip;
		this.fleeFrom = fleeFrom;
		this.fleeDist = fleeDist;
	}


	protected override void Init(){
		if (myShip.gameObject == null || fleeFrom.gameObject == null){
			SetStatus(TaskStatus.Aborted);
		}

		Debug.Log(myShip);
		Debug.Log(fleeFrom);
	}


	internal override void Update(){
		myShip.MoveInDirection((myShip.transform.position - fleeFrom.position).normalized);


		if (Vector3.Distance(myShip.transform.position, fleeFrom.position) >= fleeDist){
			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void OnSuccess(){
		myShip.ChooseNextTask();
	}
}
