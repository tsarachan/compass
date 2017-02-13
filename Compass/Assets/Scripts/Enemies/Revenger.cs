using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revenger : EnemyAttack {



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
		EventManager.Instance.Fire(new ShipSinkEvent(GetComponent<SailingShip>()));
	}


	public void PowerUpFunc(Event e){

		//first: if this ship is going to be destroyed, unregister to avoid errors
		if (e.GetType().Name == SINK_EVENT_NAME){
			ShipSinkEvent sinkEvent = e as ShipSinkEvent;

			if (sinkEvent.ship == gameObject.GetComponent<SailingShip>()){
				EventManager.Instance.Unregister<ShipSinkEvent>(powerUpFunc);
			}
		}
	}
}
