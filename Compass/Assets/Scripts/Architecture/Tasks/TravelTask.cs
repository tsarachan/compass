using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelTask : Task {

	private EnemyShip ship;
	private Vector3 destination;

	//how close the enemy must be to the destination for the task to end
	private float distTolerance = 1.0f;


	//delegate that handles ShipSinkEvents; used to deal with the traveling ship being sunk
	private ShipSinkEvent.Handler sinkFunc;


	/// <summary>
	/// This is not great! This constructor gets the necessary state--but there's a risk of stale data.
	/// 
	/// Init() deals with this issue by checking to see if the data has gone stale.
	/// </summary>
	/// <param name="ship">The ship that will be moving.</param>
	/// <param name="destination">The ship's destination point.</param>
	public TravelTask(EnemyShip ship, Vector3 destination){
		this.ship = ship;
		this.destination = destination;
	}

	protected override void Init(){
		if (ship == null) { SetStatus(TaskStatus.Aborted); }

		sinkFunc = HandleDestruction;
		EventManager.Instance.Register<ShipSinkEvent>(sinkFunc);
	}


	internal override void Update(){

		//first, make sure it still makes sense for the ship to be moving. If not, abort the task.
		if (ship.health <= 0){
			SetStatus(TaskStatus.Aborted);
			return;
		}

		//at this point it's certain that there's a functional ship
		//have it turn and move toward the destination
		ship.TurnToHeadingByTask(destination);
		ship.MoveForwardByTask();

		//this task successfully completes if the ship gets to within a reasonable distance of its destination.
		if (Vector3.Distance(ship.transform.position, destination) <= distTolerance){
			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void Cleanup(){
		EventManager.Instance.Unregister<ShipSinkEvent>(sinkFunc);
	}


	private void HandleDestruction(Event e){
		ShipSinkEvent sinkEvent = e as ShipSinkEvent;
		Debug.Log("TravelTask is handling destruction");
		if (sinkEvent.ship == ship){
			Debug.Log("TravelTask is aborting");
			Abort();
		}
	}
}
