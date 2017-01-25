using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LaunchProjectile : MonoBehaviour {

	protected string projectileName;

	protected abstract void Start();

	protected virtual void Fire(char dir){
		GameObject myProjectile = ObjectPooling.ObjectPool.GetObj(projectileName);

		myProjectile.GetComponent<Projectile>().Launch(transform.forward, transform.position);
	}
}
