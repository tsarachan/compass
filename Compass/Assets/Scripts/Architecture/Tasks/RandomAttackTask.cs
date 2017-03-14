using System.Collections;
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

		damageFunc = HandleDamage;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);

		Debug.Log("RandomAttackTask started");

		attackScript.Firing = true;
	}


	internal override void Update(){
		if (ship == null){
			SetStatus(TaskStatus.Aborted);
			return;
		}
	}


	protected override void Cleanup(){
		EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
	}


	protected override void OnSuccess(){
		attackScript.Firing = false;
	}


	private void HandleDamage(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship.gameObject.name.Contains(BOSS_OBJ)){
			if (damageEvent.damagePercent <= nextTaskPercent){
				SetStatus(TaskStatus.Succeeded);
			}
		}
	}
}
