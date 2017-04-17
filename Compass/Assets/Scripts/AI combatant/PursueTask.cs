using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueTask : Task {

	private AICombatant myShip;
	private Transform target;
	private float range;


	public PursueTask(AICombatant myShip, Transform target, float range){
		this.myShip = myShip;
		this.target = target;
		this.range = range;
	}


	protected override void Init(){
		if (myShip.gameObject == null){
			SetStatus(TaskStatus.Aborted);
		}
	}


	internal override void Update(){
		if (myShip.gameObject == null){
			SetStatus(TaskStatus.Aborted);
		}


		myShip.MoveInDirection(target.position - myShip.transform.position);

		if (Vector3.Distance(target.position, myShip.transform.position) <= range){
			SetStatus(TaskStatus.Succeeded);
		}
	}
}
