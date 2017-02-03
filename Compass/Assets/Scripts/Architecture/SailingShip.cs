﻿/*
 * 
 * Base class for seagoing vessels.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SailingShip : MonoBehaviour {

	//----------Tunable variables----------


	//how fast the ship moves, before considering weather effects
	public float forwardSpeed = 1.0f;


	//how many degrees per second the ship rotates
	public float rotationSpeed = 10.0f;


	//variables used for taking damage. See GetHit(), below.
	public int health = 10;
	public List<int> damageValues = new List<int>();


	//audio clips
	public AudioClip destroyedClip;


	//----------Internal variables----------

	//variables for accessing the current state of the weather
	protected Wind windScript;
	protected const string WEATHER_OBJ = "Weather";
	protected const string WIND_OBJ = "Wind";
	protected const string CURRENT_OBJ = "Current";
	protected bool inCurrent = false;
	protected Transform current;


	//default damage when something is wrong with damageValues
	private int defaultDamage = 1;


	//variables relating to audio
	private AudioSource audioSource;


	//variables for movement

	/*
	 * 
	 * This variable helps determine how the ship is affected by wind.
	 * 
	 * It is added to the dot product of the ship's facing and the wind's direction.
	 * 
	 * If zero, the ship will be becalmed when perpendicular to the wind, and will move at normal speed
	 * when traveling with the wind.
	 * 
	 * If 1, the ship will be becalmed when facing into the wind, will move at normal speed perpendicular to it,
	 * and will move at double speed when traveling with it.
	 * 
	 */
	public float dotAdjustment = 1.0f;


	protected Rigidbody rb;


	//initialize variables
	protected virtual void Start(){
		windScript = transform.root.Find(WEATHER_OBJ).GetComponent<Wind>();
		rb = GetComponent<Rigidbody>();
		damageValues = ShuffleList(damageValues);
		audioSource = GetComponent<AudioSource>();
	}

	#region movement

	//move ship in the direction it is facing
	protected virtual void Update(){
		rb.MovePosition(MoveForward());
	}


	/// <summary>
	/// Determines where the ship should move, given its direction, speed, and the wind.
	/// </summary>
	/// <returns>The ship's new position.</returns>
	protected virtual Vector3 MoveForward(){
		float weatherEffect = Vector3.Dot(transform.forward, windScript.GetWind()) + dotAdjustment;

		if (inCurrent){
			weatherEffect += Vector3.Dot(transform.forward, current.GetComponent<Current>().GetCurrent());
		}

		return transform.position + 
			   (transform.forward * forwardSpeed * Time.deltaTime) * 
			   weatherEffect;
	}


	//every ship must define how it turns
	protected abstract void Turn();

	#endregion


	#region current trackers

	//keeps track of whether the ship is in current
	protected virtual void OnTriggerStay(Collider other){
		if (other.gameObject.name.Contains(CURRENT_OBJ)){
			inCurrent = true;
			current = other.transform;
		}
	}

	//handles leaving current
	protected virtual void OnTriggerExit(Collider other){
		if (other.gameObject.name.Contains(CURRENT_OBJ)){
			inCurrent = false;
		}
	}

	#endregion


	#region combat
	/// <summary>
	/// Call this function whenever a ship takes damage.
	/// 
	/// When a ship is hit, it chooses the next value in a shuffled list as the amount of damage to take.
	/// This makes it possible to take damage unevenly. If every value in the list is 1, the ship will always
	/// take the same amount of damage. If the list is all zeroes and then one value equal to the ship's health,
	/// it will take no damage most of the time, but one random hit will be a critical hit that destroys it.
	/// </summary>
	public virtual void GetHit(){
		Debug.Log(gameObject.name + " got hit");

		if (damageValues.Count > 0){
			health -= damageValues[0];
			damageValues.Remove(0);
		} else {
			health -= defaultDamage;
		}

		if (health <= 0){
			GetDestroyed();
		}
	}


	protected void GetDestroyed(){
		if (!audioSource.isPlaying){
			audioSource.clip = destroyedClip;
			audioSource.Play();
		}
	}

	#endregion


	//utility function to shuffle a list's entries
	//uses the Fisher-Yates algorithm
	private List<int> ShuffleList(List<int> list){
		for (int i = 0; i < list.Count; i++) {
			int temp = list[i];
			int randomIndex = Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}

		return list;
	}
}
