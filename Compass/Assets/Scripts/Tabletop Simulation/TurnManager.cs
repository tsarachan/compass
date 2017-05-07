namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class TurnManager {


		////////////////////////////////////////////////////////////////////////
		/// How long does the simulation pause each turn?
		////////////////////////////////////////////////////////////////////////
		public float TurnDuration { get; set; }
		private float turnTimer = 0.0f;


		////////////////////////////////////////////////////////////////////////
		/// Players
		////////////////////////////////////////////////////////////////////////
		private AIPlayer ultramarinePlayer;
		private const string ULTRAMARINE_OBJ = "Ultramarines";
		private AIPlayer deathGuardPlayer;
		private const string DEATH_GUARD_OBJ = "Death Guard";
		private AIPlayer currentPlayer;
		private AIPlayer inactivePlayer;


		////////////////////////////////////////////////////////////////////////
		/// The simulation manager, so that we can track who went first each game
		////////////////////////////////////////////////////////////////////////
		private SimulationManager simManager;
		private const string MANAGER_OBJ = "Game manager";


		////////////////////////////////////////////////////////////////////////
		/// Functions
		////////////////////////////////////////////////////////////////////////


		public void Setup(float turnDuration){
			ultramarinePlayer = GameObject.Find(ULTRAMARINE_OBJ).GetComponent<AIPlayer>();
			deathGuardPlayer = GameObject.Find(DEATH_GUARD_OBJ).GetComponent<AIPlayer>();
			TurnDuration = turnDuration;
			simManager = GameObject.Find(MANAGER_OBJ).GetComponent<SimulationManager>();
			StartNewGame();
		}


		public void StartNewGame(){
			currentPlayer = ChooseRandomStartPlayer();
			inactivePlayer = GetInactivePlayer();
			simManager.CurrentStartPlayer = ReportCurrentStartPlayer();
		}


		private AIPlayer ChooseRandomStartPlayer(){
			if (OneOrNegativeOne() > 0){
				return ultramarinePlayer;
			} else {
				return deathGuardPlayer;
			}
		}

		private AIPlayer GetInactivePlayer(){
			if (currentPlayer == ultramarinePlayer){
				return deathGuardPlayer;
			} else {
				return ultramarinePlayer;
			}
		}


		private SimulationManager.Factions ReportCurrentStartPlayer(){
			if (currentPlayer == ultramarinePlayer){
				return SimulationManager.Factions.Ultramarines;
			} else {
				return SimulationManager.Factions.DeathGuard;
			}
		}


		private int OneOrNegativeOne(){
			return Random.Range(0, 2) * 2 - 1;
		}


		public void Tick(){

			turnTimer += Time.deltaTime;

			if (turnTimer >= TurnDuration){
				currentPlayer.ChooseAction();
				inactivePlayer.CleanUpBoard();
				currentPlayer = TakeTurns();
				inactivePlayer = GetInactivePlayer();
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
