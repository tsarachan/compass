using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTask : Task {

	private EnemyShip ship;
	private Transform target;


	//delegate that responds to TookDamageEvents; this is used to determine when the next task should begin
	private TookDamageEvent.Handler damageFunc;


	//used to determine whether it's the boss doing the chasing, so that the boss can have additional effects
	private const string BOSS_OBJ = "Boss";


	//delegate that responds to ShipSinkEvents; used to determine when this task should end
	private ShipSinkEvent.Handler sinkFunc;


	public ChaseTask(EnemyShip ship, Transform target){
		this.ship = ship;
		this.target = target;
	}


	protected override void Init(){
		if (ship == null || target == null){
			SetStatus(TaskStatus.Aborted);
		}

		damageFunc = HandleDamage;
		sinkFunc = HandleDestruction;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);
		EventManager.Instance.Register<ShipSinkEvent>(sinkFunc);


		//have the boss spawn more enemies in addition to chasing the player
		if (ship.gameObject.name.Contains(BOSS_OBJ)){
			SpawnTask spawnTask = ship.gameObject.GetComponent<BossBehavior>().BuildSpawnTask();
			Services.TaskManager.AddTask(spawnTask);
		}
	}


	internal override void Update(){
		if (target == null){
			SetStatus(TaskStatus.Aborted);
			return;
		}

		ship.TurnToHeadingByTask(target.position);
		ship.MoveForwardByTask();
	}


	/// <summary>
	/// Unregister for events.
	/// </summary>
	protected override void Cleanup(){
		EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
		EventManager.Instance.Unregister<ShipSinkEvent>(sinkFunc);
	}


	/// <summary>
	/// When a damage event is sent, see if it's this task's ship that was damaged. If so, and this task's ship was
	/// destroyed, stop this task.
	/// </summary>
	/// <param name="e">The TookDamageEvent.</param>
	private void HandleDamage(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship.gameObject.name == ship.gameObject.name){
			if (damageEvent.damagePercent <= 0.0f){
				SetStatus(TaskStatus.Succeeded);
			}
		}
	}


	private void HandleDestruction(Event e){
		ShipSinkEvent sinkEvent = e as ShipSinkEvent;

		if (sinkEvent.ship.gameObject == ship.gameObject){
			Abort();
		}
	}
}
