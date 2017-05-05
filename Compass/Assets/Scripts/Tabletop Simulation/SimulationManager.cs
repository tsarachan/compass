namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SimulationManager : MonoBehaviour {


		////////////////////////////////////////////////////////////////////////
		/// How many games do you want to simulate?
		////////////////////////////////////////////////////////////////////////
		public int gamesToPlay = 100;


		///////////////////////////////////////////////////////////////////////
		/// How many games has each player won so far, and how did they win?
		////////////////////////////////////////////////////////////////////////
		private int ultramarineWins = 0;
		private int deathGuardTabledWins = 0;
		private int ultramarineScoreWins = 0;

		private int deathGuardWins = 0;
		private int ultramarinesTabledWins = 0;
		private int deathGuardScoreWins = 0;


		///////////////////////////////////////////////////////////////////////
		/// Enum for describing types of victory
		////////////////////////////////////////////////////////////////////////
		public enum VictoryType { TabledOpponent, Score };



		private void Awake(){
			Services.TurnManager = new TurnManager();
			Services.TurnManager.Setup();
			Services.VictoryChecker = new VictoryChecker();
			StartNewGame();
		}


		private void StartNewGame(){
			Services.TurnManager.StartNewGame();
			Services.VictoryChecker.StartNewGame();
		}


		private void Update(){
			Services.TurnManager.Tick();
			if (Services.VictoryChecker.CheckUltramarineVictory()){
				HandleUltramarineVictory();
			} else if (Services.VictoryChecker.CheckDeathGuardVictory()){
				HandleDeathGuardVictory();
			}
		}


		private void HandleUltramarineVictory(){
			ultramarineWins++;

			if (Services.VictoryChecker.GetUltramarineVictoryType() == VictoryType.Score){
				ultramarineScoreWins++;
			} else {
				deathGuardTabledWins++;
			}
		}


		private void HandleDeathGuardVictory(){
			deathGuardWins++;

			if (Services.VictoryChecker.GetDeathGuardVictoryType() == VictoryType.Score){
				deathGuardScoreWins++;
			} else {
				ultramarinesTabledWins++;
			}
		}
	}
}
