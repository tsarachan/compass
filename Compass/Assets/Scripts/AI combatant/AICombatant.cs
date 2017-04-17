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


	//the opposing vessel this ship wants to fight
	private Transform enemy;


	//priorities
	#region priorities
	private Priority attack;
	private const string ATTACK_NAME = "attack";
	private Selector<AICombatant> attackSelector;
	private Sequence<AICombatant> launchSequence;
	private Sequence<AICombatant> pursueSequence;


	private Priority flee;
	private const string FLEE_NAME = "flee";
	private Sequence<AICombatant> fleeSequence;


	private Priority repair;
	private const string REPAIR_NAME = "repair";
	private Sequence<AICombatant> repairSequence;
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

		enemy = GameObject.Find("AI combatant blue").transform;


		myTaskManager.AddTask(new AttackTask(this, enemy, shotDelay));


		attack = MakeAttackPriority();
		flee = MakeFleePriority();
		repair = MakeRepairPriority();
	}


	private Priority MakeAttackPriority(){
		launchSequence = new Sequence<AICombatant>(new IsTargetInRange(),
												   new Attack());
		pursueSequence = new Sequence<AICombatant>(new Not<AICombatant>(new IsTargetInRange()),
												   new Pursue());

		attackSelector = new Selector<AICombatant>(launchSequence, pursueSequence);

		return new Priority(ATTACK_NAME, attackSelector, 1);
	}


	private Priority MakeFleePriority(){
		fleeSequence = new Sequence<AICombatant>(new IsTargetWithinFleeDistance(),
												 new Flee());

		return new Priority(FLEE_NAME, fleeSequence, 2);
	}


	private Priority MakeRepairPriority(){
		repairSequence = new Sequence<AICombatant>(new IsDamaged(),
												  new Repair());

		return new Priority(REPAIR_NAME, repairSequence, 3);
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


	/*----------------------------------------------------
	* Conditions, and functions they rely upon
	* ----------------------------------------------------
	*/


	private class IsTargetWithinFleeDistance : Node<AICombatant> {
		public override Result Tick (AICombatant context){
			if (context.CheckIfWithinFleeDistance()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	private bool CheckIfWithinFleeDistance(){
		return Vector3.Distance(transform.position, enemy.position) <= fleeDist ? true : false;
	}


	private class IsDamaged : Node<AICombatant> {
		public override Result Tick (AICombatant context){
			if (context.CheckIfDamaged()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	private bool CheckIfDamaged(){
		return health < startHealth? true : false;
	}


	private class IsTargetInRange : Node<AICombatant> {
		public override Result Tick (AICombatant context){
			if (context.CheckIfTargetInRange()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	private bool CheckIfTargetInRange(){
		return Vector3.Distance(transform.position, enemy.position) <= shotRange ? true : false;
	}



	/*----------------------------------------------------
	* Actions
	* ----------------------------------------------------
	*/

	private class Flee : Node<AICombatant> {
		public override Result Tick(AICombatant context){
			context.myTaskManager.AddTask(new FleeTask(context, context.enemy, context.fleeDist));

			return Result.SUCCEED;
		}
	}


	private class Repair : Node<AICombatant> {
		public override Result Tick(AICombatant context){
			context.myTaskManager.AddTask(new RepairTask(context, context.repairAmount, context.repairDelay));

			return Result.SUCCEED;
		}
	}


	private class Pursue : Node<AICombatant> {
		public override Result Tick(AICombatant context){
			context.myTaskManager.AddTask(new PursueTask(context, context.enemy, context.shotRange));

			return Result.SUCCEED;
		}
	}


	private class Attack : Node<AICombatant> {
		public override Result Tick (AICombatant context){
			context.myTaskManager.AddTask(new AttackTask(context, context.enemy, context.shotDelay));

			return Result.SUCCEED;
		}
	}




	//private class that contains the data needed to organize sequences
	private class Priority{


		public string Name { get; set; }
		public Node<AICombatant> OrderToExecute { get; set; }
		public int CurrentPriority { get; set; }


		public Priority(string name, Node<AICombatant> order, int priority){
			Name = name;
			OrderToExecute = order;
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


	public GameObject LaunchProjectile(Transform target){
		GameObject newProjectile = Instantiate(projectile,
											   transform.position,
											   Quaternion.LookRotation(target.position - transform.position,
																	   Vector3.up),
											   projectileOrganizer);
		
		newProjectile.GetComponent<Rigidbody>().AddForce((target.position - transform.position).normalized * shotForce,
														  ForceMode.Impulse);

		return newProjectile;
	}


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
}
