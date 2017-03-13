using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAttackTask : Task {

	//the percentage of the boss' health where it moves from this task to the next
	private const float transitionPercentage = 0.15f;

	private EnemyShip ship;
	private RandomAttack attackScript;


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

	protected override void Init(){
		if (ship == null || attackScript == null) {
			SetStatus(TaskStatus.Aborted);
			return;
		}

		attackScript.Firing = true;
	}


	internal override void Update(){
		if (ship == null){
			SetStatus(TaskStatus.Aborted);
			return;
		}

		if (ship.GetHealthPercentage() <= transitionPercentage){
			SetStatus(TaskStatus.Succeeded);
		}
	}


	protected override void OnSuccess(){
		attackScript.Firing = false;
	}
}
