using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public float timeBetweenShips = 3.0f;


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


	private void Start(){
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
		List<GameObject> wave2List = new List<GameObject> { Resources.Load(CURRENT_FOLLOWER) as GameObject,
															Resources.Load(CURRENT_FOLLOWER) as GameObject,
															Resources.Load(CURRENT_FOLLOWER) as GameObject };
		List<GameObject> wave3List = new List<GameObject> { Resources.Load(CIRCLER) as GameObject,
															Resources.Load(CIRCLER) as GameObject,
															Resources.Load(CIRCLER) as GameObject };

		waves.Enqueue(new Wave(wave1List, timeBetweenShips, westSpawner));
		waves.Enqueue(new Wave(wave2List, timeBetweenShips, eastSpawner));
		waves.Enqueue(new Wave(wave3List, timeBetweenShips, southSpawner));
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
	private void Update(){
		timer += Time.deltaTime;

		if (timer >= waves.Peek().Rate){
			if (waves.Peek().enemies.Count > 0){
				GameObject newEnemy = Instantiate(waves.Peek().enemies[0],
												  waves.Peek().Spawner.position,
												  Quaternion.identity,
												  enemyOrganizer) as GameObject;

				activeEnemies.Add(newEnemy);

				waves.Peek().enemies.RemoveAt(0);
			}

			timer = 0.0f;
		}
			
		if (activeEnemies.Count == 0){
			if (waves.Peek().enemies.Count <= 0){
				waves.Dequeue();
			}
		}
	} 


	/// <summary>
	/// Use this class to crate waves.
	/// </summary>
	private class Wave {

		public List<GameObject> enemies = new List<GameObject>(); //the enemies that will be created
		public float Rate { get; set; } //how many seconds will pass between each enemy
		public Transform Spawner { get; set; }


		public Wave(List<GameObject> enemies, float rate, Transform spawner){
			this.enemies = enemies;
			this.Rate = rate;
			this.Spawner = spawner;
		}
	}
}
