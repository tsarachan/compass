using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehavior : MonoBehaviour {

	void Start () {
		WaitTask waitTask = new WaitTask(2.0f);
		GameObject.Find("Game manager").GetComponent<TaskManager>().AddTask(waitTask);
	}
}
