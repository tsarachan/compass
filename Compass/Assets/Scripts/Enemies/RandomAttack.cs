using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAttack : Attack {

	//----------Tunable variables----------
	public float reloadDuration = 1.0f; //how often this enemy fires


	//----------Internal variables----------
	public bool Firing { get; set; }

	//variables for reloading
	protected float reloadTimer = 0.0f;


	//used to find the cannon port that's used for random firing
	protected Transform cannonPort;
	protected const string CANNON_PORT = "Cannon port";


	//useless char for the Fire() function, which calls for a char whose functionality this class doesn't need
	protected const char DUMMY_CHAR = 'z';

	protected override void Start(){
		Firing = false;
		cannonPort = transform.Find(CANNON_PORT);
		base.Start();
	}



	protected override void Update(){
		if (Firing){
			reloadTimer += Time.deltaTime;

			if (reloadTimer >= reloadDuration){
				Fire(DUMMY_CHAR, cannonPort.position);
			}
		}
	}


	/// <summary>
	/// This overridden version of fire shoots in a random direction.
	/// </summary>
	/// <param name="dir">Not used.</param>
	/// <param name="location">The location where the cannonball should be instantiated.</param>
	public override void Fire(char dir, Vector3 location){
		GameObject myProjectile = ObjectPooling.ObjectPool.GetObj(CANNONBALL);
		myProjectile.tag = gameObject.tag;

		myProjectile.GetComponent<Projectile>().Launch(new Vector3(0.0f,
																   Random.Range(0, 360.0f),
																   0.0f),
													   cannonPort.position);
	}
}
