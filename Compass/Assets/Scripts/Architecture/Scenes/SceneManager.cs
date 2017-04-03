using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

public class SceneManager<TTransitionData> {


	//an object that creates an explicit place for the scene system to to centered
	internal GameObject SceneRoot { get; private set; }


	//a generic dictionary for the scenes to be managed, indexed by the script all scenes will use
	private readonly Dictionary<Type, GameObject> scenes = new Dictionary<Type, GameObject>();


	//constructor for scene manager objects
	public SceneManager(GameObject root, IEnumerable<GameObject> scenePrefabs){
		SceneRoot = root;

		foreach (var prefab in scenePrefabs){
			var scene = prefab.GetComponent<Scene<TTransitionData>>();

			Assert.IsNotNull(scene, "No scene script in prefab used to initialize the scene manager");

			scenes.Add(scene.GetType(), prefab);
		}
	}


	//stack of scenes
	//maintained by the scene manager
	private readonly Stack<Scene<TTransitionData>> sceneStack = new Stack<Scene<TTransitionData>>();


	//returns the topmost scene of the stack, or null if the stack is empty
	public Scene<TTransitionData> CurrentScene {
		get { return sceneStack.Count != 0 ? sceneStack.Peek() : null; }
	}


	public void PopScene(TTransitionData data = default(TTransitionData)){

		Scene<TTransitionData> previousScene = null;
		Scene<TTransitionData> nextScene = null;

		if (sceneStack.Count != 0){
			previousScene = sceneStack.Peek();
			sceneStack.Pop();
		}

		if (sceneStack.Count != 0){
			nextScene = sceneStack.Peek();
		}

		if (nextScene != null){
			nextScene._OnEnter(data);
		}

		if (previousScene != null){
			previousScene._OnExit();
			Object.Destroy(previousScene.Root);
		}
	}


	public void PushScene<T>(TTransitionData data = default(TTransitionData)) where T : Scene<TTransitionData>{
		var previousScene = CurrentScene;
		var nextScene = GetScene<T>();

		sceneStack.Push(nextScene);
		nextScene._OnEnter(data);

		if (previousScene != null){
			previousScene._OnExit();
			previousScene.Root.SetActive(false);
		}
	}


	public void Swap<T>(TTransitionData data = default(TTransitionData)) where T : Scene<TTransitionData>{
		Scene<TTransitionData> previousScene = null;

		if (sceneStack.Count > 0){
			previousScene = sceneStack.Peek();
			sceneStack.Pop();
		}

		var nextScene = GetScene<T>();
		sceneStack.Push(nextScene);
		nextScene._OnEnter(data);

		if (previousScene != null){
			previousScene._OnExit();
			Object.Destroy(previousScene.Root);
		}
	}


	private T GetScene<T>() where T : Scene<TTransitionData>{
		GameObject prefab;

		scenes.TryGetValue(typeof(T), out prefab);
		Assert.IsNotNull(prefab, "Could not find scene prefab for scene type: " + typeof(T).Name);

		var sceneObject = Object.Instantiate(prefab);
		sceneObject.name = typeof(T).Name;
		sceneObject.transform.SetParent(SceneRoot.transform, false);
		return sceneObject.GetComponent<T>();
	}
}
