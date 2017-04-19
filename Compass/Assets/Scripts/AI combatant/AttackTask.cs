using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTask : Task {


	private AICombatant myShip;
	private Transform target;
	private float shotDelay;


	private float timer = 0.0f;


	public AttackTask(AICombatant myShip, Transform target, float shotDelay){
		this.myShip = myShip;
		this.target = target;
		this.shotDelay = shotDelay;
	}


	protected override void Init(){
		if (myShip.gameObject == null){
			SetStatus(TaskStatus.Aborted);
		}
	}


	internal override void Update(){
		timer += Time.deltaTime;

		if (timer >= shotDelay){
			myShip.LaunchProjectile(target);

			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void OnSuccess(){
		myShip.ChooseNextTask();
	}
}
