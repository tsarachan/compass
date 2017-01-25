using System.Collections;
using UnityEngine;

public class Current : MonoBehaviour {


	public Vector3 direction = -Vector3.right;
	public float speed = 1.0f;

	public Vector3 GetCurrent(){
		return direction.normalized * speed;
	}
}
