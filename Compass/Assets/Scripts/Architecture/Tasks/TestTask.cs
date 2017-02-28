using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTask : Task {

	private float timer = 0.0f;
	private float timeOver = 1.0f;

	public TestTask(float timeOver){
		this.timeOver = timeOver;
	}

	protected override void Init(){
		Debug.Log("Init() called");
	}

	internal override void Update(){
		Debug.Log("Update() called");

		timer += Time.deltaTime;

		if (timer >= timeOver){
			SetStatus(TaskStatus.Succeeded);
		}
	}

	protected override void Cleanup(){
		Debug.Log("Cleanup() called");
	}
}
