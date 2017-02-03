/*
 * 
 * Base class for projectiles such as cannonballs.
 * 
 */
using System.Collections;
using UnityEngine;

public class Projectile : ObjectPooling.Poolable {

	//----------Tunable variables----------

	//how fast the projectile moves
	public float speed = 1.0f;

	//how long the projectile flies before returning to the object pool
	public float lifetime = 3.0f;
	protected float timer = 0.0f;


	//----------Internal variables----------

	protected Rigidbody rb;
	protected const string PROJECTILE_ORGANIZER = "Projectiles";

	//tags for determining when this projectile has hit a ship
	protected const string PLAYER_TAG = "Player";
	protected const string ENEMY_TAG = "Enemy";


	//initialize variables
	protected virtual void Start(){
		rb = GetComponent<Rigidbody>();
		transform.parent = GameObject.Find(PROJECTILE_ORGANIZER).transform;
	}


	//move this projectile, and keep track of how long it has been active
	protected virtual void Update(){
		rb.MovePosition(MoveForward());
		timer = MeasureLifetime();
	}


	/// <summary>
	/// Move the projectile in the direction of its transform.forward.
	/// </summary>
	/// <returns>The projectile's new position.</returns>
	protected Vector3 MoveForward(){
		return transform.position + (transform.forward * speed * Time.deltaTime);
	}


	/// <summary>
	/// Sends out a projectile.
	/// </summary>
	/// <param name="direction">The direction of movement.</param>
	/// <param name="position">The projectile's starting position.</param>
	public virtual void Launch(Vector3 direction, Vector3 position){
		transform.forward = direction.normalized;
		transform.position = position;
		Reset();
	}


	/// <summary>
	/// Update the timer the projectile uses to decide how long it should stay active; return it to the object pool
	/// when the timer runs out.
	/// </summary>
	/// <returns>The timer's current value.</returns>
	protected virtual float MeasureLifetime(){
		float temp = timer + Time.deltaTime;

		if (temp >= lifetime){
			BackToPool();
		}

		return temp;
	}


	//reset this projectile when it comes out of the object pool
	public override void Reset(){
		timer = 0.0f; //sanity check to make sure the cannonball remains in existence for its full lifetime
		base.Reset();
	}


	/// <summary>
	/// Handle hitting another object.
	/// </summary>
	/// <param name="other">The collider struck.</param>
	protected void OnTriggerEnter(Collider other){
		if (other.gameObject.name != gameObject.name){ //discard collisions between cannonballs
			if (other.gameObject.tag != gameObject.tag &&
				(other.gameObject.tag == PLAYER_TAG ||
				other.gameObject.tag == ENEMY_TAG)){
				other.gameObject.GetComponent<SailingShip>().GetHit();

				BackToPool();
			}
		}
	}


	/// <summary>
	/// Centralizes the process for putting the cannonball back in the object pool.
	/// </summary>
	protected void BackToPool(){
		timer = 0.0f;
		ObjectPooling.ObjectPool.AddObj(gameObject);
	}
}
