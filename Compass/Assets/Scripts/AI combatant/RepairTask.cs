using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairTask : Task {


	private AICombatant myShip;
	private int repairAmount;
	private float repairDelay;


	private float timer = 0.0f;


	public RepairTask(AICombatant myShip, int repairAmount, float repairDelay){
		this.myShip = myShip;
		this.repairAmount = repairAmount;
		this.repairDelay = repairDelay;
	}


	protected override void Init(){
		if (myShip.gameObject == null){
			SetStatus(TaskStatus.Aborted);
		}
	}


	internal override void Update(){
		timer += Time.deltaTime;

		if (timer >= repairDelay){
			myShip.GetRepaired(repairAmount);

			SetStatus(TaskStatus.Succeeded);
		}
	}
}
