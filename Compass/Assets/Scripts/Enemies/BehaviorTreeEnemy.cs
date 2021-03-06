﻿using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeEnemy : EnemyShip {


	//----------Tunable variables----------

	//distance at which this enemy will stop seeking and attack
	[SerializeField] private float detectDist = 10.0f;


	//distance at which this enemy will consider itself to have successfully rammed the player during an attack
	[SerializeField] private float ramDist = 2.0f;


	//----------Internal variables----------


	//the player
	private const string PLAYER_OBJ = "Player ship";
	protected Transform player;

	private SpriteRenderer myRenderer;
	private const string MODEL_OBJ = "Model";

	private Tree<BehaviorTreeEnemy> tree;


	#region attack preparation variables

	//how many times the enemy will pulse
	[SerializeField] private int numPulses = 5;
	private int pulsesSoFar = 0;

	//how long a single pulse--full alpha to no alpha to full alpha--lasts
	private float pulseDuration = 1.0f;


	//variables to help execute the pulsing
	private float pulseTimer = 0.0f;

	#endregion

	#region fleeing variables

	private bool scared = false;

	#endregion

	#region dash attack variables

	//the ship's heading as it rushes forward
	protected Vector3 heading = new Vector3(0.0f, 0.0f, 0.0f);

	//the distance of the rush
	[SerializeField] private float rushDuration = 3.0f;
	private float rushTimer = 0.0f;


	//while rushing, multiply the ship's speed by this amount
	private float speedMult = 2.0f;


	//if closer to the player than this distance, this ship explodes, damaging the player
	float explodeDist = 2.0f;

	#endregion


	protected override void Start(){
		myRenderer = transform.Find(MODEL_OBJ).GetComponent<SpriteRenderer>();
		player = GameObject.Find(PLAYER_OBJ).transform;

		tree = new Tree<BehaviorTreeEnemy>(new Selector<BehaviorTreeEnemy>(
			new Sequence<BehaviorTreeEnemy>(
				new IsPlayerClose(), //returns success if the player is nearby
				new IsScared(), //returns success if hit during attack preparation
				new Flee() //returns success after fleeing
			),

			new Sequence<BehaviorTreeEnemy>(
				new Not<BehaviorTreeEnemy>(new IsAttackPreparationUnderway()), //returns success if not preparing to attack
				new Not<BehaviorTreeEnemy>(new IsPlayerClose()), //returns success if player is far away
				new Seek() //returns success after moving enemy toward player
			),

			new Sequence<BehaviorTreeEnemy>(
				new Not<BehaviorTreeEnemy>(new IsDonePreparing()), //returns success if enemy is not yet ready to attack
				new PrepareToAttack() //returns success after preparing to attack
			),

			new Sequence<BehaviorTreeEnemy>(
				new Not<BehaviorTreeEnemy>(new IsDoneDashing()), //returns success if enemy has not reached the destination
				new DashForward(), //returns success after dashing the enemy forward
				new TryToAttack() //returns success after attempting to ram the player
			),


			//default behavior
			new Seek()

		));

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
		base.Start();
	}

	#region condition test sandbox

	private bool CheckPlayerDetectable(){
		if (Vector3.Distance(transform.position, player.position) > detectDist){
			return false;
		} else {
			return true;
		}
	}


	private bool CheckIfAttackCharged(){
		if (pulsesSoFar >= numPulses){
			return true;
		} else {
			return false;
		}
	}


	private bool CheckIfDoneCharging(){
		rushTimer += Time.deltaTime;

		if (rushTimer >= rushDuration){
			pulsesSoFar = 0; //reset attack preparation, so that this ship can attack again
			rushTimer = 0.0f; //reset the rush timer, so that the ship can rush again
			return true;
		} else {
			return false;
		}
	}
		
	#endregion

	#region action sandbox


	/// <summary>
	/// Makes the sprite for this enemy pulse from full alpha to transparent.
	/// </summary>
	/// <returns><c>true</c>.</returns>
	private bool ChargeUpAttack(){
		pulseTimer += Time.deltaTime;

		myRenderer.color = ChangeAlpha(pulseTimer/pulseDuration);

		if (pulseTimer >= pulseDuration){
			pulsesSoFar++;
			pulseTimer = 0.0f;
		}

		return true;
	}


	private void RunFromPlayer(){
		Vector3 farFromPlayer = (player.position - transform.position).normalized * -100.0f;


		transform.rotation = Quaternion.LookRotation(TurnToHeading(farFromPlayer));
		rb.MovePosition(MoveForward());
	}


	private Color ChangeAlpha(float newAlpha){
		Color temp = myRenderer.color;
		temp.a = newAlpha;
		return temp;
	}


	private Vector3 AimDash(){
		return (player.position - transform.position).normalized;
	}


	private void Dash(){
		rb.MovePosition(transform.position + heading * forwardSpeed * speedMult);
	}

	#endregion


	protected override void Update(){
		if (Sinking && audioSource.isPlaying){
			rb.MovePosition(transform.position + -Vector3.up * sinkSpeed);
		} else if (Sinking){
			EventManager.Instance.Fire(new ShipSinkEvent(GetComponent<SailingShip>()));
			Destroy(gameObject);
		} else {
			tree.Tick(this);
		}
	}


	/// <summary>
	/// This version of GetHit adds a check against variables that are greater than zero during attack preparation,
	/// so that this enemy knows whether to flee.
	/// </summary>
	public override void GetHit(){
		if (damageValues.Count > 0){
			currentHealth -= damageValues[0];
			damageValues.RemoveAt(0);
		} else {
			currentHealth -= defaultDamage;
		}

		//send out an event; used for, e.g., the boss to decrement its health bar
		EventManager.Instance.Fire(new TookDamageEvent(this,
			GetHealthPercentage()));

		//Debug.Log("Event fired");
		SetFires();

		if (pulsesSoFar > 0 || pulseTimer > Mathf.Epsilon){
			scared = true;
		}

		if (currentHealth <= 0){
			GetDestroyed();
		}
	}


	#region nodes

	//conditions


	//checks if the ship is scared
	private class IsScared : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.scared){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	//checks if the ship is getting ready to attack
	private class IsAttackPreparationUnderway : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.pulsesSoFar > 0){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	//checks if player is too far to be detected
	private class IsPlayerClose : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if(enemy.CheckPlayerDetectable()){
				return Result.SUCCEED;
			} else {
				enemy.scared = false; //the ship is never scared when the player is far
				return Result.FAIL;
			}
		}
	}


	//checks if the attack is not yet ready
	private class IsDonePreparing : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.CheckIfAttackCharged()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	private class IsNewlyDamaged : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			return Result.FAIL;
		}
	}


	private class IsDoneDashing : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.CheckIfDoneCharging()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	//actions
	//all actions return Result.SUCCEED, so that the selector stops when the action is complete

	//turn and move toward the player
	private class Seek : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			enemy.TurnToHeadingByTask(enemy.player.position);
			enemy.MoveForwardByTask();
			return Result.SUCCEED;
		}
	}


	//pulse the enemy's sprite's alpha
	private class PrepareToAttack : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			enemy.ChargeUpAttack();

			if (enemy.pulsesSoFar == 0){
				enemy.heading = enemy.AimDash();
				enemy.TurnToHeadingByTask(enemy.player.position);
			}
			//enemy.transform.rotation = Quaternion.LookRotation(enemy.TurnToHeading(player.position));
			return Result.SUCCEED;
		}
	}


	//move away from the player
	private class Flee : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){

			//don't allow attack preparation while fleeing
			enemy.pulsesSoFar = 0;
			enemy.pulseTimer = 0.0f;
			enemy.RunFromPlayer();
			return Result.SUCCEED;
		}
	}


	//move toward the player
	private class DashForward : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			enemy.Dash();
			return Result.SUCCEED;
		}
	}


	//hit the player, and start sinking, if in close
	private class TryToAttack : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (Vector3.Distance(enemy.transform.position, enemy.player.position) < enemy.ramDist){
				enemy.player.GetComponent<SailingShip>().GetHit();
				enemy.GetDestroyed();
			}

			return Result.SUCCEED;
		}
	}

	#endregion
}
