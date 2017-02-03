using System.Collections;
using UnityEngine;

public class PlayerAttack : Attack {

	//player controls
	private KeyCode portFire = KeyCode.A;
	private KeyCode starboardFire = KeyCode.S;

	protected override void Update(){
		if (Input.GetKeyDown(portFire)){
			Fire(PORT_ATTACK);
		} else if (Input.GetKeyDown(starboardFire)){
			Fire(STARBOARD_ATTACK);
		}
	}
}
