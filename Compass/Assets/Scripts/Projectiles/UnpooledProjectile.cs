using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnpooledProjectile : MonoBehaviour {

	public GameObject MyCreator { get; set; }
	public int damage = 1;


	private const string RED_TAG = "Red";
	private const string BLUE_TAG = "Blue";


	private void OnTriggerEnter(Collider other){
		if (other.gameObject != MyCreator){
			if (other.gameObject.tag == RED_TAG || other.gameObject.tag == BLUE_TAG){
				other.gameObject.GetComponent<AICombatant>().GetHit(damage);
				Destroy(gameObject);
			}
		}
	}
}
