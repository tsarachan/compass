using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AICombatant : MonoBehaviour {



	//----------ITunable variables----------

	//starting health
	public int startHealth = 10;


	//base speed
	public float speed = 10.0f;


	//how far this ship flees from danger before recovering
	public float fleeDist = 10.0f;


	//how this ship repairs
	public float repairDelay = 1.0f; //how long passes between repair increments, in seconds
	public int repairAmount = 1; //how much this vessel repairs each increment


	//variables relating to projectiles
	public float shotForce = 2.0f; //how fast this ship's projectiles move
	public float shotDelay = 1.0f; //how quickly this ship reloads
	public float shotRange = 5.0f; //how close this ship has to be to its target in order to attack



	//----------Internal variables----------


	//the task manager and tree that will control the combatant's behavior
	private TaskManager myTaskManager;
	private Tree<AICombatant> tree;


	//this combatant's current health, as well as associated variables for displaying it to the player
	private int health;
	private Image healthBox;
	private const string CANVAS_OBJ = "Canvas";
	private const string IMAGE_OBJ = "Image";


	private Rigidbody rb;


	//the projectile AI combatants fire at each other
	private GameObject projectile;
	private const string PROJECTILE_OBJ = "Projectile prefab";
	private Transform projectileOrganizer;
	private const string PROJECTILE_ORGANIZER = "Projectiles";


	//priorities
	#region priorities
	private Priority attack;
	private const string ATTACK_NAME = "attack";


	private Priority flee;
	private const string FLEE_NAME = "flee";


	private Priority repair;
	private const string REPAIR_NAME = "repair";
	#endregion


	/*----------------------------------------------------
	 * Fundamental operation
	 * 
	 * Initialize variables and run the task manager
	 * ----------------------------------------------------
	 */

	//initialize variables
	private void Start(){
		myTaskManager = new TaskManager();

		health = startHealth;
		healthBox = transform.Find(CANVAS_OBJ).Find(IMAGE_OBJ).GetComponent<Image>();
		rb = GetComponent<Rigidbody>();
		projectile = Resources.Load(PROJECTILE_OBJ) as GameObject;
		projectileOrganizer = GameObject.Find(PROJECTILE_ORGANIZER).transform;


		myTaskManager.AddTask(new AttackTask(this, GameObject.Find("AI combatant blue").transform, shotDelay));
	}


	private void Update(){
		myTaskManager.Update();
	}


	/*----------------------------------------------------
	 * Behavior tree
	 * 
	 * Tasks use this to choose the next action
	 * ----------------------------------------------------
	 */



	public void TakeDamage(int damage){
		health -= damage;

		healthBox.fillAmount = health/startHealth;
	}


	public void GetRepaired(int repairAmount){
		health += repairAmount;

		if (health > startHealth){
			health = startHealth;
		}

		healthBox.fillAmount = health/startHealth;
	}


	//private class that contains the data needed to organize sequences
	private class Priority{


		public string Name { get; set; }
		public Sequence<AICombatant> OrderToExecute { get; set; }
		public int CurrentPriority { get; set; }


		public Priority(string name, Sequence<AICombatant> sequence, int priority){
			Name = name;
			OrderToExecute = sequence;
			CurrentPriority = priority;
		}
	}


	/*----------------------------------------------------
	 * Sandbox
	 * 
	 * Tasks use these methods to operate this ship
	 * ----------------------------------------------------
	 */


	public void MoveInDirection(Vector3 direction){
		rb.MovePosition(transform.position + direction.normalized * speed);
	}


	public bool CheckIfFullHealth(){
		return health >= startHealth ? true : false;
	}


	public GameObject Attack(Transform target){
		GameObject newProjectile = Instantiate(projectile,
											   transform.position,
											   Quaternion.LookRotation(target.position - transform.position,
																	   Vector3.up),
											   projectileOrganizer);
		
		newProjectile.GetComponent<Rigidbody>().AddForce((target.position - transform.position).normalized * shotForce,
														  ForceMode.Impulse);

		return newProjectile;
	}
}
