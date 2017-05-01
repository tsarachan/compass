namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TurnManager : MonoBehaviour {

		public float turnDuration = 1.0f;
		private float turnTimer = 0.0f;


		private AIPlayer ultramarinePlayer;
		private const string ULTRAMARINE_OBJ = "Ultramarines";
		private AIPlayer deathGuardPlayer;
		private const string DEATH_GUARD_OBJ = "Death Guard";
		private AIPlayer currentPlayer;


		private void Start(){
			ultramarinePlayer = GameObject.Find(ULTRAMARINE_OBJ).GetComponent<AIPlayer>();
			deathGuardPlayer = GameObject.Find(DEATH_GUARD_OBJ).GetComponent<AIPlayer>();
			currentPlayer = ChooseRandomStartPlayer();
		}


		private AIPlayer ChooseRandomStartPlayer(){
			if (OneOrNegativeOne() > 0){
				return ultramarinePlayer;
			} else {
				return deathGuardPlayer;
			}
		}


		private int OneOrNegativeOne(){
			return Random.Range(0, 1) * 2 - 1;
		}


		private void Update(){

			turnTimer += Time.deltaTime;

			if (turnTimer >= turnDuration){
				currentPlayer.ChooseAction();
				currentPlayer = TakeTurns();
				turnTimer = 0.0f;
			}

		}
			

		private AIPlayer TakeTurns(){
			if (currentPlayer == ultramarinePlayer){
				return deathGuardPlayer;
			} else {
				return ultramarinePlayer;
			}
		}
	}
}
