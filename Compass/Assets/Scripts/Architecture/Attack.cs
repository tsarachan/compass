using System.Collections;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

	//variables relating to the cannonballs the player shoots
	protected const string CANNONBALL = "Cannonball";


	//internal variables
	protected const char PORT_ATTACK = 'P';
	protected const char STARBOARD_ATTACK = 'S';

	
	//listen for inputs
	protected abstract void Update();


	/// <summary>
	/// Fires a cannonball to port or starboard.
	/// </summary>
	/// <param name="dir">The direction of the attack, port or starboard.</param>
	protected void Fire(char dir, Vector3 location){
		GameObject myProjectile = ObjectPooling.ObjectPool.GetObj(CANNONBALL);
		myProjectile.tag = gameObject.tag; //take the attacker's tag to avoid hitting the attacker

		if (dir == PORT_ATTACK){
			myProjectile.GetComponent<Projectile>().Launch(-transform.right, location);
		} else if (dir == STARBOARD_ATTACK){
			myProjectile.GetComponent<Projectile>().Launch(transform.right, location);
		} else {
			Debug.Log("Illegal dir: " + dir);
		}
	}
}
