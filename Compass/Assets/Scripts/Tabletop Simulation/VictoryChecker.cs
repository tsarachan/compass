namespace TabletopSimulator
{
	using System.Collections;
	using UnityEngine;

	public class VictoryChecker {


		////////////////////////////////////////////////////////////////////////
		/// Score needed to win the game
		////////////////////////////////////////////////////////////////////////
		private int winScore = 5;


		////////////////////////////////////////////////////////////////////////
		/// Variables that judge whether each player has won
		////////////////////////////////////////////////////////////////////////

		//Ultramarines win if all Death Guard defeated, or if their score >= winScore
		private int deathGuardRemaining = 0;
		private int ultramarineScore = 0;


		//Death Guard win if all Ultramarines defeated, or if their score >= winScore
		private int ultramarinesRemaining = 0;
		private int deathGuardScore = 0;


		////////////////////////////////////////////////////////////////////////
		/// Variables used to calculate the above
		////////////////////////////////////////////////////////////////////////
		private const string ULTRAMARINE_TAG = "Ultramarines";
		private const string DEATH_GUARD_TAG = "DeathGuard";


		////////////////////////////////////////////////////////////////////////
		/// Functions
		////////////////////////////////////////////////////////////////////////


		public void StartNewGame(){
			ultramarineScore = 0;
			deathGuardScore = 0;

			ultramarinesRemaining = GameObject.FindGameObjectsWithTag(ULTRAMARINE_TAG).Length;
			deathGuardRemaining = GameObject.FindGameObjectsWithTag(DEATH_GUARD_TAG).Length;
		}


		public bool CheckUltramarineVictory(){
			deathGuardRemaining = GameObject.FindGameObjectsWithTag(DEATH_GUARD_TAG).Length;

			if (ultramarineScore >= winScore ||
				deathGuardRemaining <= 0){
				return true;
			} else {
				return false;
			}
		}


		public SimulationManager.VictoryType GetUltramarineVictoryType(){
			if (ultramarineScore >= winScore){
				return SimulationManager.VictoryType.Score;
			} else {
				return SimulationManager.VictoryType.TabledOpponent;
			}
		}


		public bool CheckDeathGuardVictory(){
			ultramarinesRemaining = GameObject.FindGameObjectsWithTag(ULTRAMARINE_TAG).Length;

			if (deathGuardScore >= winScore ||
				ultramarinesRemaining <= 0){
				return true;
			} else {
				return false;
			}
		}


		public SimulationManager.VictoryType GetDeathGuardVictoryType(){
			if (deathGuardScore >= winScore){
				return SimulationManager.VictoryType.Score;
			} else {
				return SimulationManager.VictoryType.TabledOpponent;
			}
		}
	}
}
