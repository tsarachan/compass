using UnityEngine;


[CreateAssetMenu (menuName = "Prefab database")]
public class PrefabDatabase : ScriptableObject {

	[SerializeField] private GameObject[] levels;
	public GameObject[] Levels { get { return levels; } }
}
