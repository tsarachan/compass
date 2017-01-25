using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadsideAttack : LaunchProjectile {

	//variables relating to the cannonballs the player shoots
	private const string CANNONBALL = "Cannonball";

	//player controls
	private KeyCode portFire = KeyCode.A;
	private KeyCode starboardFire = KeyCode.S;


	//internal variables
	private const char PORT_ATTACK = 'P';
	private const char STARBOARD_ATTACK = 'S';

	protected override void Start(){
		projectileName = CANNONBALL;
	}
	
	// Update is called once per frame
	private void Update () {
		if (Input.GetKeyDown(portFire)){
			Fire(PORT_ATTACK);
		} else if (Input.GetKeyDown(starboardFire)){
			Fire(STARBOARD_ATTACK);
		}
	}


	protected override void Fire(char dir){
		Debug.Log("Vessel position: " + transform.position);
		GameObject myProjectile = ObjectPooling.ObjectPool.GetObj(CANNONBALL);

		if (dir == PORT_ATTACK){
			myProjectile.GetComponent<Projectile>().Launch(-transform.right, transform.position);
		} else if (dir == STARBOARD_ATTACK){
			myProjectile.GetComponent<Projectile>().Launch(transform.right, transform.position);
		} else {
			Debug.Log("Illegal dir: " + dir);
		}
	}
}
