/*
 * 
 * This script works in concert with "Poolable" to put things (e.g., enemies) into, and bring them out of, the object pool.
 * 
 * To work correctly, all objects to be in a pool must:
 * 1. inherit from Poolable, and
 * 2. be instantiated from a prefab.
 * 
 * Note that this doesn't do anything to the object other than put it in the hierarchy. The object's own script is responsible for
 * its parenting, movement after appearing in the scene, etc.
 * 
 */
namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	public class ObjectPool {

		private const string CLONE = "(Clone)";

		//these variables prevent objects from being added to pools when it's time to clear the pools and 
		//get ready for a new game
		//the ClearPools() method, below, sets it to true when the game is ending
		private static bool gameOver = false;
		public static bool GameOver{
			get { return gameOver; }
			set { gameOver = value; }
		}

		public static Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();


		/// <summary>
		/// Call this to bring an object into the playing area.
		/// 
		/// The function call is responsible for making sure the parameter is correct. This will warn you if you
		/// try to get an object with a name that doesn't exist, but it can't fix the problem.
		/// </summary>
		/// <returns>The chosen object.</returns>
		/// <param name="enemyType">The name of the object prefab.</param>
		public static GameObject GetObj(string objectType){
			GameObject obj = null; //default initialization

			//if an object of the chosen type is in the pool, get one of them
			if (objectPool.ContainsKey(objectType + CLONE)){
				if (objectPool[objectType + CLONE].Count > 0){
					obj = objectPool[objectType + CLONE].Dequeue();
					obj.GetComponent<Poolable>().Reset();
				} else {
					obj = MonoBehaviour.Instantiate(Resources.Load(objectType)) as GameObject;
				}
			}

			//if no pool exists for an object, make the object
			else {
				obj = MonoBehaviour.Instantiate(Resources.Load(objectType)) as GameObject;
			}

			if (obj == null) { Debug.Log("Unable to find object named " + objectType); }

			return obj;
		}


		/// <summary>
		/// Call this to add an object to the pool of objects.
		/// </summary>
		/// <param name="enemyType">The object to be added.</param>
		public static void AddObj(GameObject obj){
			if (!GameOver){ //sanity check to avoid carry-over pools if scene is not reloaded
				if (!objectPool.ContainsKey(obj.name)){
					objectPool.Add(obj.name, new Queue<GameObject>());
				}
					
				objectPool[obj.name].Enqueue(obj);

				obj.GetComponent<Poolable>().ShutOff();
			}
 		}


		//handle the end of the game
		public static void ClearPools(){
			GameOver = true;

			foreach (string key in objectPool.Keys){
				objectPool[key].Clear();
			}

			objectPool.Clear();
		}
	}
}
