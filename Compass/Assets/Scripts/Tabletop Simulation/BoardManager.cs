namespace TabletopSimulator
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class BoardManager {


		////////////////////////////////////////////////////////////////////////
		/// Toy soldiers
		////////////////////////////////////////////////////////////////////////
		private GameObject ultramarine;
		private GameObject deathGuard;
		private const string ULTRAMARINE_OBJ = "Ultramarine";
		private const string DEATH_GUARD_OBJ = "Death Guard";
		public int numUltramarines = 10;
		public int numDeathGuard = 10;


		////////////////////////////////////////////////////////////////////////
		/// Where each side starts setting up its toy soldiers
		/// 
		/// Toy soldiers will be in a vertical line downward from this point
		////////////////////////////////////////////////////////////////////////
		public Vector3 ultramarineSetupStart = new Vector3(-12.0f, 5.0f, 0.0f);
		public Vector3 deathGuardSetupStart = new Vector3(12.0f, 5.0f, 0.0f);


		////////////////////////////////////////////////////////////////////////
		/// The AI players--toy soldiers must be parented to their transforms
		////////////////////////////////////////////////////////////////////////
		private Transform ultramarineOrganizer;
		private Transform deathGuardOrganizer;
		private const string ULTRAMARINE_ORGANIZER = "Ultramarines";
		private const string DEATH_GUARD_ORGANIZER = "Death Guard";


		////////////////////////////////////////////////////////////////////////
		/// The simulation manager
		////////////////////////////////////////////////////////////////////////
		private SimulationManager simulationManager;
		private const string GAME_MANAGER_OBJ = "Game manager";


		public void Setup(){
			ultramarine = Resources.Load(ULTRAMARINE_OBJ) as GameObject;
			deathGuard = Resources.Load(DEATH_GUARD_OBJ) as GameObject;

			ultramarineOrganizer = GameObject.Find(ULTRAMARINE_ORGANIZER).transform;
			deathGuardOrganizer = GameObject.Find(DEATH_GUARD_ORGANIZER).transform;

			simulationManager = GameObject.Find(GAME_MANAGER_OBJ).GetComponent<SimulationManager>();
		}


		public void StartNewGame(){
			ClearBoard();

			for (int i = 0; i < numUltramarines; i++){
				Vector3 startLoc = ultramarineSetupStart;
				startLoc.y -= i;
				GameObject toySoldier = 
					MonoBehaviour.Instantiate(ultramarine, startLoc, Quaternion.identity, ultramarineOrganizer);
				toySoldier.name = ULTRAMARINE_OBJ + i.ToString() 
					+ " " + simulationManager.GamesRemaining.ToString();
			}

			for (int i = 0; i < numDeathGuard; i++){
				Vector3 startLoc = deathGuardSetupStart;
				startLoc.y -= i;
				GameObject toySoldier = 
					MonoBehaviour.Instantiate(deathGuard, startLoc, Quaternion.identity, deathGuardOrganizer);
				toySoldier.name = DEATH_GUARD_OBJ + i.ToString()
					+ " " + simulationManager.GamesRemaining.ToString();
			}
		}


		/// <summary>
		/// Get rid of all existing pieces.
		/// 
		/// The pieces are set inactive in addition to being destroyed so that AI players
		/// don't find them when getting their list of pieces; the destruction doesn't happen until the end
		/// of the update loop, and the AI players will set up during this one.
		/// </summary>
		private void ClearBoard(){
			foreach (Transform toySoldier in ultramarineOrganizer){
				toySoldier.gameObject.SetActive(false);
				MonoBehaviour.Destroy(toySoldier.gameObject);
			}

			foreach (Transform toySoldier in deathGuardOrganizer){
				toySoldier.gameObject.SetActive(false);
				MonoBehaviour.Destroy(toySoldier.gameObject);
			}
		}
	}
}
