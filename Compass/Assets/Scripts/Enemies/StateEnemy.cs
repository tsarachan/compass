using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnemy : EnemyShip {

	//----------Tunable variables----------


	//----------Internal variables----------

	private SpriteRenderer myRenderer;
	private const string MODEL_OBJ = "Model";


	private FSM<StateEnemy> myFSM;


	protected override void Start(){
		myRenderer = transform.Find(MODEL_OBJ).GetComponent<SpriteRenderer>();

		myFSM = new FSM<StateEnemy>(this);
		myFSM.TransitionTo<Seeking>();

		weatherOrganizer = GameObject.Find(WEATHER_OBJ).transform;
		base.Start();
	}


	private void Update(){
		myFSM.Update();
	}

	public void ChangeAlpha(float newAlpha){
		Color temp = myRenderer.color;
		temp.a = newAlpha;
		myRenderer.color = temp;
	}


	public IEnumerator Pulse(){

		yield break;
	}


	private class Seeking : FSM<StateEnemy>.State {


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
				Debug.Log("Transitioning to AttackPreparation");
			}
		}
	}


	private class AttackPreparation : FSM<StateEnemy>.State {

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


		public override void Cleanup(){
			EventManager.Instance.Unregister<TookDamageEvent>(damageFunc);
		}


		/// <summary>
		/// Handle damage events. When an event indicates that this state's ship is damaged while in this state,
		/// transition to the fleeing state.
		/// </summary>
		/// <param name="e">This should only ever receive TookDamageEvents.</param>
		private void HandleDamage(Event e){
			TookDamageEvent damageEvent = e as TookDamageEvent;

			if (damageEvent.ship == Context.gameObject.GetComponent<SailingShip>()){
				Parent.TransitionTo<Fleeing>();
			}
		}
	}


	private class Fleeing : FSM<StateEnemy>.State {

	}


	private class Attack : FSM<StateEnemy>.State {

	}
}
