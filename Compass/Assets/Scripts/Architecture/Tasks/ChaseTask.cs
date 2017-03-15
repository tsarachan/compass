using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTask : Task {

	private EnemyShip ship;
	private Transform target;


	//delegate that responds to TookDamageEvents; this is used to determine when the next task should begin
	private TookDamageEvent.Handler damageFunc;


	public ChaseTask(EnemyShip ship, Transform target){
		this.ship = ship;
		this.target = target;
	}


	protected override void Init(){
		if (ship == null || target == null){
			SetStatus(TaskStatus.Aborted);
		}

		damageFunc = HandleDamage;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);
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
}
