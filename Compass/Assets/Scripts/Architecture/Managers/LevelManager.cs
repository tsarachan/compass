using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager {

	private float timeBetweenShips = 3.0f;


	//a queue of all the waves that will be produced in the game
	private Queue<Wave> waves = new Queue<Wave>();

	//this keeps track of all enemies currently in the scene
	private List<GameObject> activeEnemies = new List<GameObject>();


	//names of enemies; used to populate waves
	private const string CIRCLER = "Circling enemy";
	private const string WIND_FOLLOWER = "Wind-following enemy";
	private const string CURRENT_FOLLOWER = "Current-following enemy";


	//used to parent enemies
	private Transform enemyOrganizer;
	private const string ENEMY_ORGANIZER = "Enemies";


	//the spawn points that will position the enemies
	private Transform westSpawner;
	private Transform eastSpawner;
	private Transform southSpawner;
	private const string WEST_SPAWNER = "West spawner";
	private const string EAST_SPAWNER = "East spawner";
	private const string SOUTH_SPAWNER = "South spawner";
	private const string SPAWNER_ORGANIZER = "Spawners";


	//this timer controls when the next enemy is spawned
	private float timer = 0.0f;


	public void Initialize(){
		//get a reference to the parent object for enemies
		enemyOrganizer = GameObject.Find(ENEMY_ORGANIZER).transform;

		//get references to the spawners
		westSpawner = GameObject.Find(SPAWNER_ORGANIZER).transform.Find(WEST_SPAWNER);
		eastSpawner = GameObject.Find(SPAWNER_ORGANIZER).transform.Find(EAST_SPAWNER);
		southSpawner = GameObject.Find(SPAWNER_ORGANIZER).transform.Find(SOUTH_SPAWNER);



		//create the waves
		List<GameObject> wave1List = new List<GameObject> { Resources.Load(WIND_FOLLOWER) as GameObject,
															Resources.Load(WIND_FOLLOWER) as GameObject,
															Resources.Load(WIND_FOLLOWER) as GameObject };
		List<string> wave1Spawners = new List<string> { "West spawner 1", "West spawner 2", "West spawner 3" };



		List<GameObject> wave2List = new List<GameObject> { Resources.Load(CURRENT_FOLLOWER) as GameObject,
															Resources.Load(CURRENT_FOLLOWER) as GameObject,
															Resources.Load(CURRENT_FOLLOWER) as GameObject };
		List<string> wave2Spawners = new List<string> { "East spawner 1", "East spawner 2", "East spawner 3" };



		List<GameObject> wave3List = new List<GameObject> { Resources.Load(CIRCLER) as GameObject,
															Resources.Load(CIRCLER) as GameObject,
															Resources.Load(CIRCLER) as GameObject };
		List<string> wave3Spawners = new List<string> { "South spawner 1", "South spawner 2", "South spawner 3" };



//		waves.Enqueue(new Wave(wave1List, timeBetweenShips, wave1Spawners));
//		waves.Enqueue(new Wave(wave2List, timeBetweenShips, wave2Spawners));
//		waves.Enqueue(new Wave(wave3List, timeBetweenShips, wave3Spawners));
	}



	/// <summary>
	/// Spawns waves.
	/// 
	/// Whenever the timer reaches the current wave's Rate, it instantiates the next enemy in the wave's list, 
	/// at the position of the spawner for that wave. It then removes the instantiated enemy from the list, and
	/// adds it to the list of active enemies.
	/// 
	/// When there are no active enemies and the current wave doesn't have more of them to offer,
	/// this function dequeues the current wave.
	/// </summary>
	public void Update(){
		GetEnemies();
	}


	private void GetEnemies(){
		if (waves.Count > 0){
			timer += Time.deltaTime;

			if (timer >= waves.Peek().Rate){
				if (waves.Peek().enemies.Count > 0){
					GameObject newEnemy = UnityEngine.MonoBehaviour.Instantiate(waves.Peek().enemies[0],
													  GameObject.Find(waves.Peek().spawners[0]).transform.position,
													  Quaternion.identity,
													  enemyOrganizer) as GameObject;

					activeEnemies.Add(newEnemy);

					waves.Peek().enemies.RemoveAt(0);
					waves.Peek().spawners.RemoveAt(0);
				}

				timer = 0.0f;
			}

			if (activeEnemies.Count == 0){
				if (waves.Peek().enemies.Count <= 0){
					waves.Dequeue();
				}
			}
		}
	}


	/// <summary>
	/// When an enemy registers that it's been destroyed, it calls this function 
	/// </summary>
	/// <param name="enemy">Enemy.</param>
	public void DestroyShip(GameObject enemy){
		if (activeEnemies.Contains(enemy)){
			activeEnemies.Remove(enemy);
		}
	}


	/// <summary>
	/// Other objects can use this to queue up waves of enemies
	/// </summary>
	public void AddWave(Wave wave){
		waves.Enqueue(wave);
	}
}

/// <summary>
/// Use this class to crate waves.
/// </summary>
public class Wave {

	public List<GameObject> enemies = new List<GameObject>(); //the enemies that will be created
	public float Rate { get; set; } //how many seconds will pass between each enemy
	public List<string> spawners = new List<string>(); //the spawners where enemies will be created


	public Wave(List<GameObject> enemies, float rate, List<string> spawners){
		this.enemies = enemies;
		this.Rate = rate;
		this.spawners = spawners;
	}
}
