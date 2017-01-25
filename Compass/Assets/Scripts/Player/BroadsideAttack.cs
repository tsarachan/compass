using System.Collections;
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


	//initialize variables
	protected override void Start(){
		projectileName = CANNONBALL;
	}
	
	//listen for inputs
	private void Update () {
		if (Input.GetKeyDown(portFire)){
			Fire(PORT_ATTACK);
		} else if (Input.GetKeyDown(starboardFire)){
			Fire(STARBOARD_ATTACK);
		}
	}


	/// <summary>
	/// Fires a cannonball to port or starboard.
	/// </summary>
	/// <param name="dir">The direction of the attack, port or starboard.</param>
	protected override void Fire(char dir){
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
