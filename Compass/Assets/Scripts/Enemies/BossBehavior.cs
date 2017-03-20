using System.Collections;
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

	//variables relating to the life meter
	private Image healthBar;
	private const string HEALTH_BAR_CANVAS = "Life meter canvas";
	private const string HEALTH_BAR = "Life meter";


	//this is used as part of event handling
	private TookDamageEvent.Handler damageFunc;


	//the object the boss will chase in the final phase of the boss fight
	private const string PLAYER_OBJ = "Player ship";


	//a reference to the GameManager; used to stop the game when the boss is destroyed
	private GameManager gameManager;
	private const string MANAGER_OBJ = "Game manager";


	/// <summary>
	/// Initialize variables and set up tasks.
	/// </summary>
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

		ChaseTask chaseTask = new ChaseTask(this, GameObject.Find(PLAYER_OBJ).transform);
		randomAttackTask.Then(chaseTask);
		Services.TaskManager.AddTask(travelTask);


		gameManager = GameObject.Find(MANAGER_OBJ).GetComponent<GameManager>();
	}


	/// <summary>
	/// Put together a task that spawns waves of enemies.
	/// </summary>
	/// <returns>The task.</returns>
	public SpawnTask BuildSpawnTask(){
		List<GameObject> enemiesToSpawn = BuildEnemyList();
		List<string> spawners = BuildSpawnerList();

		Debug.Assert(enemiesToSpawn.Count == spawners.Count);

		Wave wave = new Wave(enemiesToSpawn, rate, spawners);

		return new SpawnTask(transform, wave);
	}


	/// <summary>
	/// Use publicly-set variables to create a list of enemies to spawn as part of the spawn task.
	/// </summary>
	/// <returns>The list.</returns>
	private List<GameObject> BuildEnemyList(){
		List<GameObject> temp = new List<GameObject>();

		for (int i = 0; i < spawnsPerWave; i++){
			temp.Add(Resources.Load(spawnedEnemy) as GameObject);
		}

		Debug.Assert(temp.Count > 0);

		return temp;
	}


	/// <summary>
	/// Create a list of "spawners" for the spawn task. In this case, the spawner is always the boss itself.
	/// </summary>
	/// <returns>The list, which is this gameobject's name repeated.</returns>
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
	protected override void Update(){
		if (Sinking && audioSource.isPlaying){
			rb.MovePosition(transform.position + -Vector3.up * sinkSpeed);
		} else if (Sinking){
			EventManager.Instance.Fire(new ShipSinkEvent(GetComponent<SailingShip>()));
			gameManager.GameHasStarted = false;
			Destroy(gameObject);
		}
	}


	/// <summary>
	/// Handle damage events. When an event indicates that this ship is damaged, reduce the life bar. If the boss
	/// has been destroyed, unregister for the damage events.
	/// </summary>
	/// <param name="e">This should only ever receive TookDamageEvents.</param>
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
