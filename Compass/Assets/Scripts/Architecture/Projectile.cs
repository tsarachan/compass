using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : ObjectPooling.Poolable {

	public float speed = 1.0f;


	public float lifetime = 3.0f;
	protected float timer = 0.0f;


	//internal variables
	protected Rigidbody rb;
	protected const string PROJECTILE_ORGANIZER = "Projectiles";


	protected virtual void Start(){
		rb = GetComponent<Rigidbody>();
		transform.parent = GameObject.Find(PROJECTILE_ORGANIZER).transform;
	}


	protected virtual void Update(){
		rb.MovePosition(MoveForward());
		timer = MeasureLifetime();
	}


	protected Vector3 MoveForward(){
		return transform.forward * speed * Time.deltaTime;
	}


	public virtual void Launch(Vector3 direction, Vector3 position){
		transform.forward = direction;
		transform.position = position;
		Reset();
	}


	protected virtual float MeasureLifetime(){
		float temp = timer + Time.deltaTime;

		if (temp >= lifetime){
			ObjectPooling.ObjectPool.AddObj(gameObject);
			temp = 0.0f;
		}

		return temp;
	}


	public override void Reset(){
		timer = 0.0f;
		base.Reset();
	}
}
