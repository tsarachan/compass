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
		private int gamesRemaining = 0;
		public int GamesRemaining{
			get { return gamesRemaining; }
			private set { gamesRemaining = value; }
		}


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


		////////////////////////////////////////////////////////////////////////
		/// Players
		////////////////////////////////////////////////////////////////////////
		private const string ULTRAMARINE_PLAYER = "Ultramarines";
		private const string DEATH_GUARD_PLAYER = "Death Guard";


		private void Awake(){
			Services.TurnManager = new TurnManager();
			Services.TurnManager.Setup();
			Services.VictoryChecker = new VictoryChecker();
			Services.BoardManager = new BoardManager();
			Services.BoardManager.Setup();
		}


		private void StartNewGame(){
			GamesRemaining--;
			Services.TurnManager.StartNewGame();
			Services.VictoryChecker.StartNewGame();
			Services.BoardManager.StartNewGame();
			GameObject.Find(ULTRAMARINE_PLAYER).GetComponent<AIPlayer>().Setup();
			GameObject.Find(DEATH_GUARD_PLAYER).GetComponent<AIPlayer>().Setup();
		}


		private void BeginSimulation(){
			GamesRemaining = gamesToPlay;
			StartNewGame();
		}


		private void Update(){
			if (Input.anyKeyDown){
				BeginSimulation();
			}

			if (gamesRemaining > 0){
				Services.TurnManager.Tick();
				if (Services.VictoryChecker.CheckUltramarineVictory()){
					HandleUltramarineVictory();
					StartNewGame();
				} else if (Services.VictoryChecker.CheckDeathGuardVictory()){
					HandleDeathGuardVictory();
					StartNewGame();
				}
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
