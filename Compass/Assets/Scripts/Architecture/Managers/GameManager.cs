using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {


	private void Awake () {

		Services.TaskManager = new TaskManager();
		Services.LevelManager = new LevelManager();
		Services.LevelManager.Initialize();
	}


	public void Update(){
		Services.TaskManager.Update();
		Services.LevelManager.Update();
	}
}
