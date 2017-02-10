/*
 * 
 * Base class for attacking with projectiles.
 * 
 */
using System.Collections;
using UnityEngine;

public abstract class Attack : MonoBehaviour {

	//----------tunable variables----------
	public float minCannonPitch = 0.5f;
	public float maxCannonPitch = 1.5f;
	public float minCannonVolume = 0.25f;
	public float maxCannonVolume = 1.0f;


	//----------internal variables----------
	protected const char PORT_ATTACK = 'P';
	protected const char STARBOARD_ATTACK = 'S';


	//variables relating to the cannonballs the player shoots
	protected const string CANNONBALL = "Cannonball";


	//for playing a sound when ships attack
	protected AudioClip cannonSound;
	protected AudioSource audioSource;


	protected virtual void Start(){
		cannonSound = Resources.Load("Audio/SFX/Cannon") as AudioClip;
		audioSource = GetComponent<AudioSource>();
	}

	
	//use the update loop to direct each ship to attack
	protected abstract void Update();


	/// <summary>
	/// Fires a cannonball to port or starboard.
	/// </summary>
	/// <param name="dir">The direction of the attack, port or starboard.</param>
	/// <param name="location">The location where the cannonball should be instantiated.</param>
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

		if (!audioSource.isPlaying){
			PlayVariableSound(cannonSound);
		}
	}

	protected void PlayVariableSound(AudioClip clip){
		audioSource.pitch = Random.Range(minCannonPitch, maxCannonPitch);
		audioSource.volume = Random.Range(minCannonVolume, maxCannonVolume);
		audioSource.clip = clip;
		audioSource.Play();
	}
}
