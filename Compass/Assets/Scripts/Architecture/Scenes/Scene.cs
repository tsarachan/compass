using UnityEngine;

public class Scene<TTransitionData> : MonoBehaviour {

	public GameObject Root { get { return gameObject; } }


	//"hidden" enter and exit methods that activate or deactivate the gameobject, so that subclasses need not do so
	internal void _OnEnter(TTransitionData data){
		Root.SetActive(true);
		OnEnter(data);
	}


	internal void _OnExit(){
		Root.SetActive(false);
		OnExit();
	}


	internal virtual void OnEnter(TTransitionData data) { }
	internal virtual void OnExit() { }
}
