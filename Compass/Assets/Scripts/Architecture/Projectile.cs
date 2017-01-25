using System.Collections;
using UnityEngine;

public class Projectile : ObjectPooling.Poolable {

	//how fast the projectile moves
	public float speed = 1.0f;

	//how long the projectile flies before returning to the object pool
	public float lifetime = 3.0f;
	protected float timer = 0.0f;


	//internal variables
	protected Rigidbody rb;
	protected const string PROJECTILE_ORGANIZER = "Projectiles";


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
		Debug.Log("Projectile position: " + transform.position);
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
			ObjectPooling.ObjectPool.AddObj(gameObject);
			temp = 0.0f;
		}

		return temp;
	}


	//reset this projectile when it comes out of the object pool
	public override void Reset(){
		timer = 0.0f;
		base.Reset();
	}
}
