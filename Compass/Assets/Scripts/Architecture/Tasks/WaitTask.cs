using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTask : Task {

	private float timer = 0.0f;
	private float duration = 1.0f;

	public WaitTask(float duration){
		this.duration = duration;
	}

	protected override void Init(){
		Debug.Log("Init() called");
	}

	internal override void Update(){
		Debug.Log("Update() called");

		timer += Time.deltaTime;

		if (timer >= duration){
			SetStatus(TaskStatus.Succeeded);
		}
	}

	protected override void Cleanup(){
		Debug.Log("Cleanup() called");
	}
}
