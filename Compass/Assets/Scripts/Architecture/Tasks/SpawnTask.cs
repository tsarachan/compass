using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTask : Task {

	//the percentage of the boss' damage where this task will stop and the next will begin
	public float nextTaskPercent = 0.5f;


	private Transform start; //the transform of the enemy that will spawn more enemies
	private GameObject spawnEnemy; //the enemies that will be spawned by this task


	//delegate that responds to TookDamageEvents; this is used to determine when the next task should begin
	private TookDamageEvent.Handler damageFunc;
	private const string BOSS_OBJ = "Boss";


	public SpawnTask(Transform start, GameObject spawnEnemy){
		this.start = start;
		this.spawnEnemy = spawnEnemy;
	}


	protected override void Init(){
		if (start == null) { SetStatus(TaskStatus.Aborted); }

		damageFunc = HandleDamage;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);
	}


	protected override void Cleanup(){
		EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
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
