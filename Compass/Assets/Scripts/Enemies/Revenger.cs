/*
 * 
 * This is a variant of EnemyAttack that powers up as other enemies are sunk. Each sunk enemy causes this enemy
 * to fire an additional cannonball with each broadside, up to three cannonballs.
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revenger : EnemyAttack {

	//----------Internal variables----------

	//extra cannon ports the revenger ship uses to power up
	protected Transform cannonPort2;
	protected const string CANNON_PORT_2 = "Cannon port 2";
	protected Transform cannonPort3;
	protected const string CANNON_PORT_3 = "Cannon port 3";

	//a list of cannon ports
	private Dictionary<Transform, bool> ports = new Dictionary<Transform, bool>();

	//the delegate that responds to ShipSinkEvents--in this case, it powers the ship up
	private ShipSinkEvent.Handler powerUpFunc;
	private const string SINK_EVENT_NAME = "ShipSinkEvent";


	//initialize variables
	protected override void Start(){
		base.Start();

		cannonPort2 = transform.Find(CANNON_PORT_2);
		cannonPort3 = transform.Find(CANNON_PORT_3);

		ports = new Dictionary<Transform, bool>() { 
			{ cannonPort, true },
			{ cannonPort2, false},
			{ cannonPort3, false }
		};

		powerUpFunc = new ShipSinkEvent.Handler(PowerUpFunc);
		EventManager.Instance.Register<ShipSinkEvent>(powerUpFunc);
	}


	/// <summary>
	/// When this ship is instructed to power up, switch on another bank of cannons if any are available.
	/// </summary>
	/// <param name="e">The event that was published.</param>
	public void PowerUpFunc(Event e){
		//first: if this ship is going to be destroyed, unregister to avoid errors
		if (e.GetType().Name == SINK_EVENT_NAME){ // better tha nthis: a debug.assert to confirm that the right type of event arrived
			//also better: use the type-safe event manager, which will throw a compiler error if the wrong event arrives.
			ShipSinkEvent sinkEvent = e as ShipSinkEvent;

			if (sinkEvent.ship == gameObject.GetComponent<SailingShip>()){
				EventManager.Instance.Unregister<ShipSinkEvent>(powerUpFunc);
			}
		}

		//second: go through the dictionary of ports. If one is turned off, switch it on and then stop.
		foreach (Transform port in ports.Keys){
			if (ports[port] == false){
				ports[port] = true;

				return;
			}
		}
	}


	/// <summary>
	/// This is the heart of the revenger's powering up: instead of firing one cannonball, the revenger fires
	/// from every active port. More ports become active as the revenger powers up.
	/// </summary>
	protected override void ChooseAttack(){
		if (transform.InverseTransformPoint(player.position).x < 0.0f){
			RaycastHit hitInfo;

			foreach (Transform port in ports.Keys){
				if (ports[port] == true){

					//don't shoot if the enemy will hit a friendly
					if (Physics.SphereCast(port.position, sphereRadius, -transform.right, out hitInfo, enemyLayerMask)){
						if (hitInfo.transform.tag == ENEMY_TAG){
							return;
						}
					}

					Fire(PORT_ATTACK, port.position);

				}
			}
		} else if (transform.InverseTransformPoint(player.position).x > 0.0f){
			RaycastHit hitInfo;

			foreach (Transform port in ports.Keys){
				if (ports[port] == true){
					//don't shoot if the enemy will hit a friendly
					if (Physics.SphereCast(port.position, sphereRadius, transform.right, out hitInfo, enemyLayerMask)){
						if (hitInfo.transform.tag == ENEMY_TAG){
							return;
						}
					}

					Fire(STARBOARD_ATTACK, port.position);
				}
			}
		}
	}
}
