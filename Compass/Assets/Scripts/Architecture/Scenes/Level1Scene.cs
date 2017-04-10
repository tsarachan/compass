using UnityEngine;

public class Level1Scene : Scene<TransitionData> {

	private void Update(){
		Services.LevelManager.Update();
		Services.TaskManager.Update();
	}
}
