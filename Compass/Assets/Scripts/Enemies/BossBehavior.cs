using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : EnemyShip {


	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);


	protected override void Start(){
		base.Start();

		TravelTask travelTask = new TravelTask(this, destination);
		GameObject.Find("Game manager").GetComponent<TaskManager>().AddTask(travelTask);
	}


	//Update() and Turn() are intentionally blank; tasks will control the behavior of this enemy
	protected override void Update(){ }

	protected override void Turn(){ }





}
