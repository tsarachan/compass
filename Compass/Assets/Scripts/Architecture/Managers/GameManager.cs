using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {


	public bool GameHasStarted { get; set; }

	/// <summary>
	/// Create and/or initialize services, as appropriate.
	/// 
	/// This must occur first, so this should be the ONLY script with an Awake() function.
	/// </summary>
	private void Awake () {
		Services.TaskManager = new TaskManager();
		Services.LevelManager = new LevelManager();
		Services.LevelManager.Initialize();

		GameHasStarted = true;
	}


	/// <summary>
	/// Direct services to update.
	/// </summary>
	public void Update(){
		if (GameHasStarted){
			Services.TaskManager.Update();
			Services.LevelManager.Update();
		}
	}
}
