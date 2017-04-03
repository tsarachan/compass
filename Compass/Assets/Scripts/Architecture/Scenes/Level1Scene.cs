using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Scene : Scene<TransitionData> {

	private void Update(){
		Services.LevelManager.Update();
		Services.TaskManager.Update();
	}
}
