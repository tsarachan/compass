using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEnemy : EnemyShip {

	//----------Tunable variables----------


	//----------Internal variables----------

	private SpriteRenderer myRenderer;
	private const string MODEL_OBJ = "Model";


	private FSM<GameOverEnemy> myFSM;


	protected override void Start(){
		myRenderer = transform.Find(MODEL_OBJ).GetComponent<SpriteRenderer>();

		myFSM = new FSM<GameOverEnemy>(this);
		myFSM.TransitionTo<Seeking>();

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
		base.Start();
	}


	protected override void Update(){
		if (Sinking && audioSource.isPlaying){
			rb.MovePosition(transform.position + -Vector3.up * sinkSpeed);
		} else if (Sinking){
			EventManager.Instance.Fire(new ShipSinkEvent(GetComponent<SailingShip>()));
			myFSM.Clear();
			Services.SceneManager.Swap<GameOverScene>(new TransitionData(true));
			Destroy(gameObject);
		} else {
			myFSM.Update();
		}
	}

	public void ChangeAlpha(float newAlpha){
		Color temp = myRenderer.color;
		temp.a = newAlpha;
		myRenderer.color = temp;
	}


	public IEnumerator Pulse(){

		yield break;
	}


	private class Seeking : FSM<GameOverEnemy>.State {


		//distance at which this enemy will stop seeking and attack
		private float detectDist = 10.0f;


		//the object this enemy will chase
		private const string PLAYER_OBJ = "Player ship";
		private Transform player;

		ChaseTask chaseTask;


		public override void Init(){
			player = GameObject.Find(PLAYER_OBJ).transform;
		}


		public override void OnEnter(){
			chaseTask = new ChaseTask(Context.GetComponent<EnemyShip>(),
				GameObject.Find(PLAYER_OBJ).transform);

			Services.TaskManager.AddTask(chaseTask);
		}


		public override void Update(){
			if (Vector3.Distance(Context.transform.position, player.position) <= detectDist){
				Parent.TransitionTo<AttackPreparation>();
				chaseTask.Abort();
			}
		}
	}


	private class AttackPreparation : FSM<GameOverEnemy>.State {

		//how many times the enemy will pulse
		private int numPulses = 5;
		private int pulsesSoFar = 0;

		//how long a single pulse--full alpha to no alpha to full alpha--lasts
		private float pulseDuration = 1.0f;


		//variables to help execute the pulsing
		private float timer = 0.0f;


		//the context's renderer--used to pulse
		private SpriteRenderer contextRenderer;


		//delegate that handles damage events
		private TookDamageEvent.Handler damageFunc;


		public override void Init(){
			damageFunc = HandleDamage;
			EventManager.Instance.Register<TookDamageEvent>(damageFunc);
			contextRenderer = Context.gameObject.GetComponent<SpriteRenderer>();
		}


		public override void Update(){
			timer += Time.deltaTime;

			Context.ChangeAlpha(timer/pulseDuration);

			if (timer >= pulseDuration){
				timer = 0.0f;
				pulsesSoFar++;
			}

			if (pulsesSoFar >= numPulses){
				Parent.TransitionTo<Attack>();
			}
		}


		public override void OnExit(){
			EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
			timer = 0.0f;
		}


		public override void Cleanup(){
			EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
			timer = 0.0f;
		}


		/// <summary>
		/// Handle damage events. When an event indicates that this state's ship is damaged while in this state,
		/// transition to the fleeing state.
		/// </summary>
		/// <param name="e">This should only ever receive TookDamageEvents.</param>
		private void HandleDamage(Event e){
			TookDamageEvent damageEvent = e as TookDamageEvent;

			if (damageEvent.ship == Context.gameObject.GetComponent<SailingShip>()){
				Context.ChangeAlpha(1.0f);
				Parent.TransitionTo<Fleeing>();
			}
		}
	}


	private class Fleeing : FSM<GameOverEnemy>.State {


		//while fleeing, multiply the ship's speed by this amount
		private float speedMult = 2.0f;


		//how far from the player the ship wants to flee
		//This must be greater than the Seeking state's detectDist, or else the ship won't actually flee
		private float fleeDist = 15.0f;


		//the player's transform, to flee from
		private Transform player;
		private const string PLAYER_OBJ = "Player ship";


		//increase the ship's speed as it flees
		public override void Init(){
			Context.forwardSpeed *= speedMult;
			player = GameObject.Find(PLAYER_OBJ).transform;
		}


		public override void Update(){
			Vector3 farFromPlayer = (player.position - Context.transform.position).normalized * -100.0f;


			Context.transform.rotation = Quaternion.LookRotation(Context.TurnToHeading(farFromPlayer));
			Context.rb.MovePosition(Context.MoveForward());

			if (Vector3.Distance(Context.transform.position, player.position) >= fleeDist){
				Parent.TransitionTo<Seeking>();
			}
		}

		//return the ship's speed to normal
		public override void OnExit(){
			Context.forwardSpeed /= speedMult;
		}
	}


	private class Attack : FSM<GameOverEnemy>.State {


		//the ship's heading as it rushes forward, and related variables
		private Vector3 heading = new Vector3(0.0f, 0.0f, 0.0f);
		private const string PLAYER_OBJ = "Player ship";


		//the distance of the rush
		private Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
		float totalDist = 10.0f;


		//while rushing, multiply the ship's speed by this amount
		private float speedMult = 2.0f;


		//if closer to the player than this distance, this ship explodes, damaging the player
		float explodeDist = 2.0f;
		private Transform player;


		public override void Init(){
			Context.forwardSpeed *= speedMult;
			player = GameObject.Find(PLAYER_OBJ).transform;
			heading = (player.position - Context.transform.position).normalized;
			start = Context.transform.position;
		}


		public override void Update(){
			Context.transform.rotation = Quaternion.LookRotation(Context.TurnToHeading(Context.transform.position +
				heading));
			Context.rb.MovePosition(Context.MoveForward());

			if (Vector3.Distance(Context.transform.position, player.position) <= explodeDist){
				player.GetComponent<SailingShip>().GetHit();
				Context.GetDestroyed();
			}

			if (Vector3.Distance(Context.transform.position, start) >= totalDist){
				Parent.TransitionTo<Seeking>();
			}
		}


		public override void OnExit(){
			Context.forwardSpeed /= speedMult;
		}
	}
}
