using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTask : Task {


	//the percentage of the boss' damage where this task will stop and the next will begin
	private float nextTaskPercent = 0.5f;

	//the waves this enemy will spawn
	private Wave enemyWave;

	private float spawnTimer = 0.0f;


	private Transform start; //the transform of the enemy that will spawn more enemies
	private GameObject spawnEnemy; //the enemies that will be spawned by this task


	//delegate that responds to TookDamageEvents; this is used to determine when the next task should begin
	private TookDamageEvent.Handler damageFunc;
	private const string BOSS_OBJ = "Boss";


	public SpawnTask(Transform start, Wave enemyWave){
		this.start = start;
		this.enemyWave = enemyWave;
	}


	protected override void Init(){
		if (start == null) { SetStatus(TaskStatus.Aborted); }

		damageFunc = HandleDamage;
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);

		Debug.Log("SpawnTask() begun");
		Services.LevelManager.AddWave(enemyWave);
	}


	//add a new wave of enemies to the level manager when the previous wave is exhausted
	internal override void Update(){
		spawnTimer += Time.deltaTime;

		if (spawnTimer >= enemyWave.Rate * enemyWave.enemies.Count){
			Services.LevelManager.AddWave(enemyWave);
		}
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
