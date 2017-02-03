using System.Collections;
using UnityEngine;

public class PlayerAttack : Attack {

	//player controls
	private KeyCode portFire = KeyCode.A;
	private KeyCode starboardFire = KeyCode.S;


	//----------Internal variables----------
	private Transform cannonPort;
	private const string CANNON_PORT = "Cannon port";


	private void Start(){
		cannonPort = transform.Find(CANNON_PORT);
	}

	protected override void Update(){
		if (Input.GetKeyDown(portFire)){
			Fire(PORT_ATTACK, cannonPort.position);
		} else if (Input.GetKeyDown(starboardFire)){
			Fire(STARBOARD_ATTACK, cannonPort.position);
		}
	}
}
