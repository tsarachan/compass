using System.Collections;
using UnityEngine;

public class SetupScript : MonoBehaviour {

	public int numCannonballs = 10;
	private const string CANNONBALL = "Cannonball";


	//add objects to the object pool
	private void Start () {
		for (int i = 0; i < numCannonballs; i++){
			GameObject cannonball = Instantiate(Resources.Load(CANNONBALL)) as GameObject;
			ObjectPooling.ObjectPool.AddObj(cannonball);
		}
	}
}
