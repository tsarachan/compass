using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private const string PREFAB_DATABASE_OBJ = "Prefab database";



	public bool GameHasStarted { get; set; }

	/// <summary>
	/// Create and/or initialize services, as appropriate.
	/// 
	/// This must occur first, so this should be the ONLY script with an Awake() function.
	/// </summary>
	private void Awake () {
		Services.TaskManager = new TaskManager();
		Services.LevelManager = new LevelManager();
		Services.PrefabDatabase = Resources.Load<PrefabDatabase>(PREFAB_DATABASE_OBJ);
		Services.SceneManager = new SceneManager<TransitionData>(gameObject, Services.PrefabDatabase.Levels);
		Services.SceneManager.PushScene<TitleScene>();

		GameHasStarted = false;
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
