/*
 * 
 * Everything that uses the object pool inherits from this script.
 * 
 */

namespace ObjectPooling
{
	using UnityEngine;
	using System.Collections;

	public abstract class Poolable : MonoBehaviour {



		/// <summary>
		/// Call this function to restore default values when an enemy comes out of the pool and into play.
		/// 
		/// Call this *before* the enemy is moved into position, so that everything is in a predictable state when the enemy's own script takes over.
		/// </summary>
		public virtual void Reset(){
			gameObject.SetActive(true);
		}

		/// <summary>
		/// Call this function when an enemy is going back to the pool.
		/// </summary>
		public virtual void ShutOff(){
			gameObject.SetActive(false); //shut the enemy off so that it doesn't move, etc.
		}
	}
}
