using UnityEngine;
using System.Collections;

/**
 * Component used by the ObjectPoolManager to mark its pooled GameObjects.
 * //TODO: find a better way of storing this information without breaking 
 * the prefab connection (possible solution: a list of gameobjects managed by the GameObjectPoolMgr?)
 */

public class PoolObject : MonoBehaviour {
	
	/**
	 * stores a reference to the original GameObject instance (or prefab instance) from where this game object was created 
	 * (Note: used by the pool manager to determine to which object pool this object belongs to)
	 */
	public GameObject srcInstance;
}