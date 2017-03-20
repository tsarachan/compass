using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateEnemy : EnemyShip {

	//----------Tunable variables----------


	private FSM<StateEnemy> myFSM;


	private void Start(){
		myFSM = new FSM<StateEnemy>(this);
		myFSM.TransitionTo<Seeking>();
	}


	private void Update(){
		myFSM.Update();
	}

	public void Test(){

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


		public override void Init(){
			player = GameObject.Find(PLAYER_OBJ).transform;
		}


		public override void OnEnter(){
			Services.TaskManager.AddTask(new ChaseTask(Context.GetComponent<EnemyShip>(),
													   GameObject.Find(PLAYER_OBJ).transform));
		}


		public override void Update(){
			if (Vector3.Distance(Context.transform.position, player.position) <= detectDist){
				Parent.TransitionTo<AttackPreparation>();
			}
		}
	}


	private class AttackPreparation : FSM<StateEnemy>.State {


		//how many times the enemy will pulse
		private int numPulses = 5;


		//how long a single pulse--full alpha to no alpha to full alpha--lasts
		private float pulseDuration = 1.0f;

		private TookDamageEvent.Handler damageFunc;


		public override void Init(){
			damageFunc = HandleDamage;
			EventManager.Instance.Register<TookDamageEvent>(damageFunc);
		}


		public override void OnEnter(){
			Context.Test();
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
}
