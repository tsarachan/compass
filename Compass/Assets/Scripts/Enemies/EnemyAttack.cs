using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack {


	//----------Tunable variables----------
	public float reloadDuration = 1.0f; //how often this enemy fires


	//----------Internal variables----------


	//variables for reloading
	private float reloadTimer = 0.0f;

	//variables used to find the player
	private const string PLAYER_OBJ = "Player ship";
	private Transform player;


	//initialize variables
	private void Start(){
		player = GameObject.Find(PLAYER_OBJ).transform;
	}


	//reload and fire
	protected override void Update(){
		reloadTimer += Time.deltaTime;

		if (reloadTimer >= reloadDuration){
			ChooseAttack();
			reloadTimer = 0.0f;
		}
	}


	//determine whether the player is to the left or right, and fire accordingly
	private void ChooseAttack(){
		if (transform.InverseTransformPoint(player.position).x < 0.0f){
			Fire(PORT_ATTACK);
		} else if (transform.InverseTransformPoint(player.position).x > 0.0f){
			Fire(STARBOARD_ATTACK);
		}
	}
}
