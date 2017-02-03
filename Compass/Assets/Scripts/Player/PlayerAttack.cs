/*
 * 
 * This class handles the player's inputs for attacking.
 * 
 */
using System.Collections;
using UnityEngine;

public class PlayerAttack : Attack {

	//player controls
	private KeyCode portFire = KeyCode.A;
	private KeyCode starboardFire = KeyCode.S;


	//----------Internal variables----------
	private Transform cannonPort;
	private const string CANNON_PORT = "Cannon port";


	//initialize variables
	private void Start(){
		cannonPort = transform.Find(CANNON_PORT);
	}


	//detect input, and attack accordingly
	protected override void Update(){
		if (Input.GetKeyDown(portFire)){
			Fire(PORT_ATTACK, cannonPort.position);
		} else if (Input.GetKeyDown(starboardFire)){
			Fire(STARBOARD_ATTACK, cannonPort.position);
		}
	}
}
