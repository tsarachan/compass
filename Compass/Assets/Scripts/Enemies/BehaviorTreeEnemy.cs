using BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeEnemy : EnemyShip {


	//----------Tunable variables----------

	//distance at which this enemy will stop seeking and attack
	[SerializeField] private float detectDist = 10.0f;


	//----------Internal variables----------


	//the player
	private const string PLAYER_OBJ = "Player ship";
	protected Transform player;

	private SpriteRenderer myRenderer;
	private const string MODEL_OBJ = "Model";

	private Tree<BehaviorTreeEnemy> tree;


	#region alpha-pulse variables

	//how many times the enemy will pulse
	[SerializeField] private int numPulses = 5;
	private int pulsesSoFar = 0;

	//how long a single pulse--full alpha to no alpha to full alpha--lasts
	private float pulseDuration = 1.0f;


	//variables to help execute the pulsing
	private float pulseTimer = 0.0f;

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
				new IsPlayerFar(), //returns success if player is far away
				new Seek() //returns success after moving enemy toward player
			),

			new Sequence<BehaviorTreeEnemy>(
				new IsNewlyDamaged(), //returns success if there's been a recent damage event
				new IsStillPreparing(), //returns success if the enemy is not yet ready to attack
				new Flee()
			),

			new Sequence<BehaviorTreeEnemy>(
				new IsStillPreparing(), //returns success if enemy is not yet ready to attack
				new PrepareToAttack() //returns success after preparing to attack
			),

			new Sequence<BehaviorTreeEnemy>(
				new IsStillDashing(), //returns success if enemy has not reached the destination
				new DashForward() //returns success after dashing the enemy forward
			),

			new Seek()

		));

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
		base.Start();
	}

	#region test sandbox

	private bool CheckPlayerDetectable(){
		if (Vector3.Distance(transform.position, player.position) > detectDist){
			return false;
		} else {
			return true;
		}
	}


	private bool CheckIfAttackCharged(){
		Debug.Log("pulsesSoFar == " + pulsesSoFar + ", numPulses == " + numPulses);
		if (pulsesSoFar >= numPulses){
			Debug.Log("returning true");
			return true;
		} else {
			Debug.Log("returning false");
			return false;
		}
	}


	private bool CheckIfDoneCharging(){
		if (rushTimer >= rushDuration){
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
		Debug.Log("Dashing; heading == " + heading + ", forwardSpeed == " + forwardSpeed + ", speedMult == " + speedMult);
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


	#region nodes

	//conditions


	//checks if player is too far to be detected
	private class IsPlayerFar : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if(!enemy.CheckPlayerDetectable()){
				return Result.SUCCEED;
			} else {
				return Result.FAIL;
			}
		}
	}


	//checks if the attack is not yet ready
	private class IsStillPreparing : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.CheckIfAttackCharged()){
				return Result.FAIL;
			} else {
				return Result.SUCCEED;
			}
		}
	}


	private class IsNewlyDamaged : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			return Result.FAIL;
		}
	}


	private class IsStillDashing : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			if (enemy.CheckIfDoneCharging()){
				Debug.Log("Done dashing");
				return Result.FAIL;
			} else {
				Debug.Log("Still dashing");
				return Result.SUCCEED;
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
			enemy.heading = enemy.AimDash();
			enemy.TurnToHeadingByTask(enemy.player.position);
			Debug.Log("Charging attack");
			//enemy.transform.rotation = Quaternion.LookRotation(enemy.TurnToHeading(player.position));
			return Result.SUCCEED;
		}
	}


	//move away from the player
	private class Flee : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			enemy.RunFromPlayer();
			Debug.Log("Fleeing");
			return Result.SUCCEED;
		}
	}


	//move toward the player
	private class DashForward : Node<BehaviorTreeEnemy>{
		public override Result Tick(BehaviorTreeEnemy enemy){
			enemy.Dash();
			Debug.Log("Dashing");
			return Result.SUCCEED;
		}
	}

	#endregion
}
