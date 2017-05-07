namespace TabletopSimulator
{
	using System.Collections;
	using TMPro;
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


		////////////////////////////////////////////////////////////////////////
		/// How long should each simulated turn last?
		////////////////////////////////////////////////////////////////////////
		public float turnDuration = 0.1f;


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


		///////////////////////////////////////////////////////////////////////
		/// Variables for tracking whether the first mover won
		////////////////////////////////////////////////////////////////////////
		public enum Factions { Ultramarines, DeathGuard };
		public Factions CurrentStartPlayer { get; set; }


		////////////////////////////////////////////////////////////////////////
		/// Players
		////////////////////////////////////////////////////////////////////////
		private const string ULTRAMARINE_PLAYER = "Ultramarines";
		private const string DEATH_GUARD_PLAYER = "Death Guard";



		////////////////////////////////////////////////////////////////////////
		/// UI
		////////////////////////////////////////////////////////////////////////
		private TextMeshProUGUI ultramarineText;
		private const string ULTRAMARINE_TEXT_OBJ = "Ultramarine win info";
		private TextMeshProUGUI deathGuardText;
		private const string DEATH_GUARD_TEXT_OBJ = "Death Guard win info";
		private const string ULTRAMARINE_WIN_TOTAL = "Ultramarine wins: ";
		private const string DEATH_GUARD_WIN_TOTAL = "Death Guard wins: ";
		private const string TABLED = "Tabled opponent: ";
		private const string SCORE = "Score: ";
		private TextMeshProUGUI firstMoverText;
		private const string FIRST_MOVER_TEXT_OBJ = "First mover win info";
		private const string FIRST_MOVER_WIN_TOTAL = "First mover wins: ";
		private const string UM_FIRST_MOVER_TOTAL = "UM first mover wins: ";
		private const string DG_FIRST_MOVER_TOTAL = "DG first mover wins: ";
		private int firstMoverWins = 0;
		private int umFirstMoverWins = 0;
		private int dgFirstMoverWins = 0;


		////////////////////////////////////////////////////////////////////////
		/// Control whether the simulation has begun
		////////////////////////////////////////////////////////////////////////
		private bool simHasStarted = false;


		private void Awake(){
			Services.TurnManager = new TurnManager();
			Services.TurnManager.Setup(turnDuration);
			Services.VictoryChecker = new VictoryChecker();
			Services.BoardManager = new BoardManager();
			Services.BoardManager.Setup();
			ultramarineText = GameObject.Find(ULTRAMARINE_TEXT_OBJ).GetComponent<TextMeshProUGUI>();
			deathGuardText = GameObject.Find(DEATH_GUARD_TEXT_OBJ).GetComponent<TextMeshProUGUI>();
			firstMoverText = GameObject.Find(FIRST_MOVER_TEXT_OBJ).GetComponent<TextMeshProUGUI>();
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
			if (gamesRemaining >= 0 && simHasStarted){
				Services.TurnManager.Tick();
				if (Services.VictoryChecker.CheckUltramarineVictory()){
					HandleUltramarineVictory();
					ChangeUltramarineText();
					firstMoverText.text = CheckForFirstPlayerVictory(Factions.Ultramarines);
					StartNewGame();
				} else if (Services.VictoryChecker.CheckDeathGuardVictory()){
					HandleDeathGuardVictory();
					ChangeDeathGuardText();
					firstMoverText.text = CheckForFirstPlayerVictory(Factions.DeathGuard);
					StartNewGame();
				}
			}

			if (Input.anyKeyDown){
				BeginSimulation();
				simHasStarted = true;
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


		private void ChangeUltramarineText(){
			ultramarineText.text = ULTRAMARINE_WIN_TOTAL + ultramarineWins.ToString() + "\n"
								   + TABLED + deathGuardTabledWins.ToString() + "\n"
								   + SCORE + ultramarineScoreWins.ToString();
		}


		private void ChangeDeathGuardText(){
			deathGuardText.text = DEATH_GUARD_WIN_TOTAL + deathGuardWins.ToString() + "\n"
								  + TABLED + ultramarinesTabledWins.ToString() + "\n"
								  + SCORE + deathGuardScoreWins.ToString();
		}


		private string CheckForFirstPlayerVictory(Factions faction){
			if (faction == CurrentStartPlayer){
				firstMoverWins++;

				if (faction == Factions.Ultramarines){
					umFirstMoverWins++;
				} else {
					dgFirstMoverWins++;
				}
			}

			return FIRST_MOVER_WIN_TOTAL + firstMoverWins.ToString() + "\n"
				+ UM_FIRST_MOVER_TOTAL + umFirstMoverWins.ToString() + "\n"
				+ DG_FIRST_MOVER_TOTAL + dgFirstMoverWins.ToString();
		}
	}
}
