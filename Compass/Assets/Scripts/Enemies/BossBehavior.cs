using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : EnemyShip {


	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);


	protected override void Start(){
		base.Start();

		TravelTask travelTask = new TravelTask(this, destination);
		//create and schedule all the tasks here
		Services.TaskManager.AddTask(travelTask);
	}

	protected override void Turn(){ }


	//this Update() is intentionally limited, since this ship is run by tasks instead of by its own update loop
	protected override void Update(){ }



}
