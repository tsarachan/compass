using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour {


	private void Awake () {

		Services.TaskManager = new TaskManager();
	}


	public void Update(){
		Services.TaskManager.Update();
	}
}
