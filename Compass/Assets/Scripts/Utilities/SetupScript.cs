using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupScript : MonoBehaviour {

	public int numCannonballs = 10;
	private const string CANNONBALL = "Cannonball";


	private void Start () {

		//add objects to object pool
		for (int i = 0; i < numCannonballs; i++){
			GameObject cannonball = Instantiate(Resources.Load(CANNONBALL)) as GameObject;
			ObjectPooling.ObjectPool.AddObj(cannonball);
		}
		
	}
}
