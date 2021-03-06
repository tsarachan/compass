﻿using BehaviorTree;
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


	//priorities
	public int attackPriority = 1;
	public int fleePriority = 2;
	public int repairPriority = 3;



	//----------Internal variables----------


	//the task manager and tree that will control the combatant's behavior
	private TaskManager myTaskManager;
	private Tree<AICombatant> tree;


	//this combatant's current health, as well as associated variables for displaying it to the player
	private int health;
	private Image healthBox;
	private const string CANVAS_OBJ = "Canvas";
	private const string HEALTH_IMAGE_OBJ = "Health fill";


	private Rigidbody rb;


	//the projectile AI combatants fire at each other
	private GameObject projectile;
	private const string PROJECTILE_OBJ = "Projectile prefab";
	private Transform projectileOrganizer;
	private const string PROJECTILE_ORGANIZER = "Projectiles";


	//the opposing vessel this ship wants to fight
	private Transform enemy;


	//ships use these to pick their enemies
	private const string RED = "Red";
	private const string BLUE = "Blue";
	private const string AI_COMBATANT_OBJ = "AI combatant ";


	//flags
	private Image flag1;
	private Image flag2;
	private Image flag3;
	private const string FLAG_1_OBJ = "Top priority flag";
	private const string FLAG_2_OBJ = "Second priority flag";
	private const string FLAG_3_OBJ = "Third priority flag";


	//UI for priority
	private Text attackUI;
	private Text fleeUI;
	private Text repairUI;
	private const string ATTACK_UI_OBJ = "Attack priority";
	private const string FLEE_UI_OBJ = "Flee priority";
	private const string REPAIR_UI_OBJ = "Repair priority";
	private const string CONTROLS_UI_OBJ = "Controls";


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
		healthBox = transform.Find(CANVAS_OBJ).Find(HEALTH_IMAGE_OBJ).GetComponent<Image>();
		rb = GetComponent<Rigidbody>();
		projectile = Resources.Load(PROJECTILE_OBJ) as GameObject;
		projectileOrganizer = GameObject.Find(PROJECTILE_ORGANIZER).transform;

		enemy = ChooseEnemy();


		myTaskManager.AddTask(new AttackTask(this, enemy, shotDelay));


		flag1 = transform.Find(CANVAS_OBJ).Find(FLAG_1_OBJ).GetComponent<Image>();
		flag2 = transform.Find(CANVAS_OBJ).Find(FLAG_2_OBJ).GetComponent<Image>();
		flag3 = transform.Find(CANVAS_OBJ).Find(FLAG_3_OBJ).GetComponent<Image>();


		attack = MakeAttackPriority();
		flee = MakeFleePriority();
		repair = MakeRepairPriority();

		attackUI = transform.root.Find(CONTROLS_UI_OBJ).Find(ATTACK_UI_OBJ).GetComponent<Text>();
		fleeUI = transform.root.Find(CONTROLS_UI_OBJ).Find(FLEE_UI_OBJ).GetComponent<Text>();
		repairUI = transform.root.Find(CONTROLS_UI_OBJ).Find(REPAIR_UI_OBJ).GetComponent<Text>();
	}


	private Transform ChooseEnemy(){
		Transform closestEnemy = transform;;
		float closestDistance = 100000000.0f; //nonsense initialization; should always be larger than the play area

		if (gameObject.tag == RED){
			GameObject[] blueTeam = GameObject.FindGameObjectsWithTag(BLUE);

			foreach (GameObject enemy in blueTeam){
				if (Vector3.Distance(transform.position, enemy.transform.position) < closestDistance){
					closestDistance = Vector3.Distance(transform.position, enemy.transform.position);
					closestEnemy = enemy.transform;
				}
			}
		} else if (gameObject.tag == BLUE){
			GameObject[] redTeam = GameObject.FindGameObjectsWithTag(RED);

			foreach (GameObject enemy in redTeam){
				if (Vector3.Distance(transform.position, enemy.transform.position) < closestDistance){
					closestDistance = Vector3.Distance(transform.position, enemy.transform.position);
					closestEnemy = enemy.transform;
				}
			}
		}

		Debug.Assert(closestEnemy != transform);

		return closestEnemy;
	}


	private Priority MakeAttackPriority(){
		launchSequence = new Sequence<AICombatant>(new IsTargetInRange(),
												   new Attack());
		pursueSequence = new Sequence<AICombatant>(new Not<AICombatant>(new IsTargetInRange()),
												   new Pursue());

		attackSelector = new Selector<AICombatant>(launchSequence, pursueSequence);

		return new Priority(ATTACK_NAME, attackSelector, attackPriority, Color.red);
	}


	private Priority MakeFleePriority(){
		fleeSequence = new Sequence<AICombatant>(new IsTargetWithinFleeDistance(),
												 new Flee());

		return new Priority(FLEE_NAME, fleeSequence, fleePriority, Color.yellow);
	}


	private Priority MakeRepairPriority(){
		repairSequence = new Sequence<AICombatant>(new IsDamaged(),
												  new Repair());

		return new Priority(REPAIR_NAME, repairSequence, repairPriority, Color.blue);
	}


	private void Update(){
		myTaskManager.Update();
	}


	public void ChooseNextTask(){
		//Debug.Log("ChooseNextTask() called");

		Priority priority1;
		Priority priority2;
		Priority priority3;
		List<Priority> priorities = new List<Priority>() { attack, flee, repair };

		priorities = PutPrioritiesInOrder(priorities);


		ReorderFlags(priorities);


		//Debug.Log(priorities);

		tree = new Tree<AICombatant>(new Selector<AICombatant>(priorities[0].OrderToExecute,
															   priorities[1].OrderToExecute,
															   priorities[2].OrderToExecute));

		tree.Tick(this);
	}


	/// <summary>
	/// Orders a list of priorities by their CurrentPriority field, high to low
	/// </summary>
	/// <returns>A list of priorities, ordered high to low.</returns>
	/// <param name="priorityList">The list to be put in order.</param>
	private List<Priority> PutPrioritiesInOrder(List<Priority> priorityList){
		List<Priority> temp = new List<Priority>();

		int totalRuns = priorityList.Count;

		for (int i = 0; i < totalRuns; i++){
			int maxSoFar = -1; //nonsense initialization; all CurrentPriorities should always be higher than this
			Priority nextPriority = new Priority();

			for (int j = 0; j < priorityList.Count; j++){
				if (priorityList[j].CurrentPriority > maxSoFar){
					nextPriority = priorityList[j];
					maxSoFar = priorityList[j].CurrentPriority;
				}
			}

			Debug.Assert(nextPriority.Name != "Default");

			//Debug.Log("Adding " + nextPriority.Name);
			temp.Add(nextPriority);
			priorityList.Remove(nextPriority);
		}

		Debug.Assert(temp.Count > 0);

		return temp;
	}


	private void ReorderFlags(List<Priority> priorities){
		Debug.Assert(priorities.Count == 3);

		flag1.color = priorities[0].FlagColor;
		flag2.color = priorities[1].FlagColor;
		flag3.color = priorities[2].FlagColor;
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
			if (context.gameObject.name.Contains(RED)){
				//Debug.Log("IsTargetWithinFleeDistance called");
			}
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
			if (context.gameObject.name.Contains(RED)){
				//Debug.Log("IsDamaged called");
			}
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
			if (context.gameObject.name.Contains(RED)){
				//Debug.Log("IsTargetInRange called");
			}
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
			if (context.gameObject.name.Contains(RED)){
				Debug.Log("Flee called");
			}
			context.myTaskManager.AddTask(new FleeTask(context, context.enemy, context.fleeDist));
			context.flee.CurrentPriority = 0; //reset the flee priority after successfully fleeing

			return Result.SUCCEED;
		}
	}


	private class Repair : Node<AICombatant> {
		public override Result Tick(AICombatant context){
			if (context.gameObject.name.Contains(RED)){
				Debug.Log("Repair called");
			}
			context.myTaskManager.AddTask(new RepairTask(context, context.repairAmount, context.repairDelay));

			return Result.SUCCEED;
		}
	}


	private class Pursue : Node<AICombatant> {
		public override Result Tick(AICombatant context){
			if (context.gameObject.name.Contains(RED)){
				Debug.Log("Pursue called");
			}
			context.myTaskManager.AddTask(new PursueTask(context, context.enemy, context.shotRange));

			return Result.SUCCEED;
		}
	}


	private class Attack : Node<AICombatant> {
		public override Result Tick (AICombatant context){
			if (context.gameObject.name.Contains(RED)){
				Debug.Log("Attack called");
			}
			context.myTaskManager.AddTask(new AttackTask(context, context.enemy, context.shotDelay));

			return Result.SUCCEED;
		}
	}




	//private class that contains the data needed to organize sequences
	private class Priority{


		public string Name { get; set; }
		public Node<AICombatant> OrderToExecute { get; set; }
		public int CurrentPriority { get; set; }
		public Color FlagColor { get; set; }


		public Priority(string name, Node<AICombatant> order, int priority, Color flagColor){
			Name = name;
			OrderToExecute = order;
			CurrentPriority = priority;
			FlagColor = flagColor;
		}


		public Priority(){
			Name = "Default";
			OrderToExecute = null;
			CurrentPriority = 0;
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
		newProjectile.GetComponent<UnpooledProjectile>().MyCreator = gameObject;
		
		newProjectile.GetComponent<Rigidbody>().AddForce((target.position - transform.position).normalized * shotForce,
														  ForceMode.Impulse);

		return newProjectile;
	}


	public void TakeDamage(int damage){
		health -= damage;

		healthBox.fillAmount = (float)health/startHealth;
	}


	public void GetRepaired(int repairAmount){
		health += repairAmount;

		if (health > startHealth){
			health = startHealth;
		}

		healthBox.fillAmount = (float)health/startHealth;
	}


	/*----------------------------------------------------
	 * Controls
	 * 
	 * Used to manipulate the ship's priorities
	 * ----------------------------------------------------
	 */

	public void AttackPlus(){
		attack.CurrentPriority++;

		UpdateUI();
	}

	public void AttackMinus(){
		attack.CurrentPriority--;

		if (attack.CurrentPriority < 0){
			attack.CurrentPriority = 0;
		}

		UpdateUI();
	}


	public void FleePlus(){
		flee.CurrentPriority++;

		UpdateUI();
	}

	public void FleeMinus(){
		flee.CurrentPriority--;

		if (flee.CurrentPriority < 0){
			flee.CurrentPriority = 0;
		}

		UpdateUI();
	}


	public void RepairPlus(){
		repair.CurrentPriority++;

		UpdateUI();
	}

	public void RepairMinus(){
		repair.CurrentPriority--;

		if (repair.CurrentPriority < 0){
			repair.CurrentPriority = 0;
		}

		UpdateUI();
	}


	/*----------------------------------------------------
	 * Utility
	 * 
	 * Other methods this ship needs to operate
	 * ----------------------------------------------------
	 */


	public void GetHit(int damage){
		TakeDamage(damage);

		flee.CurrentPriority += damage;

		UpdateUI();
	}


	public void UpdateUI(){
		if (gameObject.tag == RED){
			attackUI.text = attack.CurrentPriority.ToString();
			fleeUI.text = flee.CurrentPriority.ToString();
			repairUI.text = repair.CurrentPriority.ToString();
		}
	}
}
