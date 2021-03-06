﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAttackTask : Task {

	//the percentage of the boss' health where it moves from this task to the next
	private const float nextTaskPercent = 0.15f;

	private EnemyShip ship;
	private RandomAttack attackScript;


	//delegate that responds to TookDamageEvents; this is used to determine when the next task should begin
	private TookDamageEvent.Handler damageFunc;
	private const string BOSS_OBJ = "Boss";


	//delegate that responds to ShipSinkEvents; used to handle the boss sinking during battle
	private ShipSinkEvent.Handler sinkFunc;


	/// <summary>
	/// Constructor for random attack tasks. This is at risk of stale data; Init(), below, checks for that
	/// possibility.
	/// </summary>
	/// <param name="ship">The enemy that will be randomly firing.</param>
	/// <param name="attackScript">The random attack script that will actually shoot the cannon.</param>
	public RandomAttackTask(EnemyShip ship, RandomAttack attackScript){
		this.ship = ship;
		this.attackScript = attackScript;
	}


	/// <summary>
	/// Check to make sure the state is not stale. Assuming it is not, register for TookDamageEvents so that this task
	/// can tell when it should stop, and then start the ship this task is associated with firing randomly.
	/// </summary>
	protected override void Init(){
		if (ship == null || attackScript == null) {
			SetStatus(TaskStatus.Aborted);
			return;
		}

		damageFunc = HandleDamageRandom;
		sinkFunc = HandleDestruction;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);
		EventManager.Instance.Register<ShipSinkEvent>(sinkFunc);

		attackScript.Firing = true;
	}


	//unregister for events
	protected override void Cleanup(){
		EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
		EventManager.Instance.Unregister<ShipSinkEvent>(sinkFunc);
	}


	//stop the ship from firing at random when this task ends successfully.
	protected override void OnSuccess(){
		attackScript.Firing = false;
	}


	/// <summary>
	/// Handle damage events. If the damaged ship was this task's ship, stop this task if the damage has reached
	/// the prescribed amount. Then additionally check to see whether the ship was destroyed; if so, abort this
	/// task.
	/// </summary>
	/// <param name="e">The TookDamageEvent.</param>
	private void HandleDamageRandom(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship.gameObject.name == ship.gameObject.name){
			if (damageEvent.damagePercent <= nextTaskPercent){
				SetStatus(TaskStatus.Succeeded);
			}

			if (damageEvent.damagePercent <= 0.0f){
				SetStatus(TaskStatus.Aborted);
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
