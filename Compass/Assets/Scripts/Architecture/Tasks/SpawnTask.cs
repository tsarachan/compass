using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTask : Task {

	private Transform start; //the transform of the enemy that will spawn more enemies
	private List<GameObject> spawnEnemies; //the enemies that will be spawned by this task


	public SpawnTask(Transform start, List<GameObject> spawnEnemies){
		this.start = start;
		this.spawnEnemies = spawnEnemies;
	}


	protected override void Init(){
		if (start == null) { SetStatus(TaskStatus.Aborted); }
	}
}
