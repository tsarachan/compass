namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SimulationManager : MonoBehaviour {

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
				Debug.Log("Ultramarines win!");
			} else if (Services.VictoryChecker.CheckDeathGuardVictory()){
				Debug.Log("Death Guard wins!");
			}
		}
	}
}
