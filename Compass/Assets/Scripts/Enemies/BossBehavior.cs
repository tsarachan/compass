﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : EnemyShip {

	//----------Tunable variables----------


	//the location in world space where the boss goes at the start of the battle
	public Vector3 destination = new Vector3(0.0f, 0.0f, 0.0f);
	public string spawnedEnemy = "Wind-following enemy"; //the enemy this enemy spawns during the spawn phase
	public float rate = 3.0f; //the rate at which enemies will spawn during the spawn phase
	public int spawnsPerWave = 3; //the number of enemies in each wave during the spawn phase


	//----------Internal variables----------
	private Image healthBar;
	private const string HEALTH_BAR_CANVAS = "Life meter canvas";
	private const string HEALTH_BAR = "Life meter";

	private TookDamageEvent.Handler damageFunc;

	protected override void Start(){
		base.Start();

		healthBar = transform.Find(HEALTH_BAR_CANVAS).Find(HEALTH_BAR).GetComponent<Image>();

		damageFunc = new TookDamageEvent.Handler(HandleDamage);
		EventManager.Instance.Register<TookDamageEvent>(damageFunc);

		TravelTask travelTask = new TravelTask(this, destination);


		SpawnTask spawnTask = BuildSpawnTask();
		travelTask.Then(spawnTask);
		//create and schedule all the tasks here
		RandomAttackTask randomAttackTask = new RandomAttackTask(this, GetComponent<RandomAttack>());
		spawnTask.Then(randomAttackTask);
		Services.TaskManager.AddTask(travelTask);
	}


	private SpawnTask BuildSpawnTask(){
		List<GameObject> enemiesToSpawn = BuildEnemyList();
		List<string> spawners = BuildSpawnerList();

		Debug.Assert(enemiesToSpawn.Count == spawners.Count);

		Wave wave = new Wave(enemiesToSpawn, rate, spawners);

		return new SpawnTask(transform, wave);
	}


	private List<GameObject> BuildEnemyList(){
		List<GameObject> temp = new List<GameObject>();

		for (int i = 0; i < spawnsPerWave; i++){
			temp.Add(Resources.Load(spawnedEnemy) as GameObject);
		}

		Debug.Assert(temp.Count > 0);

		return temp;
	}


	private List<string> BuildSpawnerList(){
		List<string> temp = new List<string>();

		for (int i = 0; i < spawnsPerWave; i++){
			temp.Add(gameObject.name);
		}

		Debug.Assert(temp.Count > 0);

		return temp;
	}

	protected override void Turn(){ }


	//this Update() is intentionally limited, since this ship is run by tasks instead of by its own update loop
	protected override void Update(){ }


	private void HandleDamage(Event e){
		TookDamageEvent damageEvent = e as TookDamageEvent;

		if (damageEvent.ship == this){
			healthBar.fillAmount = damageEvent.damagePercent;

			if (damageEvent.damagePercent <= 0.0f){
				EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
			}
		}
	}
}