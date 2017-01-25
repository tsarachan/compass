/*
 * 
 * This is a base class for attacking.
 * 
 * To create a basic attack, initialize projectileName in Start(), and then call Fire() with any char.
 * 
 * Fire() takes a char as an argument so that child classes can use it, if necessary.
 * 
 */

using System.Collections;
using UnityEngine;

public abstract class LaunchProjectile : MonoBehaviour {

	protected string projectileName;

	protected abstract void Start();

	protected virtual void Fire(char dir){
		GameObject myProjectile = ObjectPooling.ObjectPool.GetObj(projectileName);

		myProjectile.GetComponent<Projectile>().Launch(transform.forward, transform.position);
	}
}
