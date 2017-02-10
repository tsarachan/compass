using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : Attack {


	//----------Tunable variables----------
	public float reloadDuration = 1.0f; //how often this enemy fires

	//this is the size of the sphere enemies cast to see if they're in danger of hitting a friendly with a broadside
	public float sphereRadius = 1.0f;


	//----------Internal variables----------


	//variables for reloading
	private float reloadTimer = 0.0f;

	//variables used to find the player
	private const string PLAYER_OBJ = "Player ship";
	private Transform player;

	private Transform cannonPort;
	private const string CANNON_PORT = "Cannon port";


	//these are used to find enemies, so that the enemy ships don't fire on each other
	private int enemyLayer = 8;
	private int enemyLayerMask;
	private const string ENEMY_TAG = "Enemy";


	//initialize variables
	protected override void Start(){
		player = GameObject.Find(PLAYER_OBJ).transform;
		cannonPort = transform.Find(CANNON_PORT);
		enemyLayerMask = 1 << enemyLayer; //bitwise math to create a layermask
		base.Start();
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
			RaycastHit hitInfo;

			//don't shoot if the enemy will hit a friendly
			if (Physics.SphereCast(cannonPort.position, sphereRadius, -transform.right, out hitInfo, enemyLayerMask)){
				if (hitInfo.transform.tag == ENEMY_TAG){
					return;
				}
			}

			Fire(PORT_ATTACK, cannonPort.position);
		} else if (transform.InverseTransformPoint(player.position).x > 0.0f){
			RaycastHit hitInfo;

			//don't shoot if the enemy will hit a friendly
			if (Physics.SphereCast(cannonPort.position, sphereRadius, transform.right, out hitInfo, enemyLayerMask)){
				if (hitInfo.transform.tag == ENEMY_TAG){
					return;
				}
			}

			Fire(STARBOARD_ATTACK, cannonPort.position);
		}
	}
}
