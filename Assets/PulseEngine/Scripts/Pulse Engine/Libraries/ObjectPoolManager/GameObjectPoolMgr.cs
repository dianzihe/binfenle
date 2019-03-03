/**
 * 
 * GameObjectPoolMgr version 1.0.6b
 * by Florin Covit
 * 
 * Manages pools of GameObjects of different capacities, storing different instances of GameObjects in separate pools.
 * The user can request an object from any pool by specifying the original GameObject instance (for example the prefab GameObject) and an instance of
 * that GameObject will be returned (without allocating memory at the time of the request, depending on the capacity of the pool). 
 * By default the class is set to auto create pools of objects if you request an object instance that doesn't already have an object pool created.
 * It also auto-sizes the capacity of the pools by creating at request time extra object instances for object pools that are empty. (can lead to performance hickups if used every frame)
 *
 * Note: For optimal performace you should always create the object pools manually with the object capacities you estimate to be enough for your needs.
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPoolMgr : MonoBehaviour
{	
	
	// ===== Public members =====
	[System.NonSerialized]
	public static bool isBeingDestroyed;
	
	/**
	 * if set to true, when requesting an object from an empty pool it will automatically allocate an extra instance for that object and return it, thus extending the object capacity of that pool
	 * if set to false, when requesting an object from an empty pool it will return null
	 */
	[System.NonSerialized]
	public bool autoSizePools = true;
	
	/**
	 * if set to true, when requesting an object from a inexisting object pool the object pool will be created with a default capacity and an instance for the specified object type will be returned
	 * if false, null will be returned when requesting an object from a inexisting object pool
	 */
	[System.NonSerialized]
	public bool autoCreatePools = true;
	[System.NonSerialized]
	public int defaultPoolCapacity = 10;
	
	// the inactive game object containing all the inactive instances of all the game objects from all the pools managed by the pool manager
	[System.NonSerialized]
	public GameObject poolManagerContainer;
	
	// ===== Static members =====	
	private static GameObjectPoolMgr instance = null;
	
		
	// ====== Private members ======
	private Dictionary<GameObject, Queue<GameObject>> poolContainer = new Dictionary<GameObject, Queue<GameObject>>(30);
	
	
	public static GameObjectPoolMgr Instance {
		get {			
			if (instance == null) {
				isBeingDestroyed = false;
					
				GameObject gameObj = GameObject.Find("PoolManager");
				
				if (gameObj == null) {
					gameObj = new GameObject("PoolManager");
				}
				
				GameObjectPoolMgr tempInst = gameObj.GetComponent<GameObjectPoolMgr>();
				if (tempInst == null)
					instance = gameObj.AddComponent<GameObjectPoolMgr>();
				else
					instance = tempInst;
			}
			
			return instance;
		}
	}
			
	void Awake() {
		// create an inactive parent game object container for all the inactive game objects from the pools of objects
		// Note: this will help the engine easilly bypass all the inactive game objects stored in the pools
		if (poolManagerContainer == null)  {
			poolManagerContainer = new GameObject("_PoolManagerContainer_");
			poolManagerContainer.transform.position = Vector3.zero;
			poolManagerContainer.transform.localPosition = Vector3.zero;
			poolManagerContainer.SetActive(false);
		}
	}
	
	
	/**
	 * Creates a new object pool for the specified object with the initial default pool capacity. (specified by the @defaultPoolCapacity member)
	 * If an object pool has already been created, its capacity will be extended by the
	 * default pool capacity number.
	 * @param obj the object for which a new object pool will be created
	 * @param capacity the number of object instances to be created in the object pool
	 * 
	 * @see CreatePoolForObject(GameObject obj, int capacity)
	 */
	public void CreatePoolForObject(GameObject obj) {
		CreatePoolForObject(obj, defaultPoolCapacity);
	}
	
	/**
	 * Creates a new object pool for the specified object with the specified capacity. 
	 * If an object pool has already been created, its capacity will be extended by the
	 * specified capacity number (a number of @capacity new instances will be pre-allocated in the existing pool).
	 * You must NEVER destroy the main GameObject instance (the source object) for which you created an object pool.
	 * @param obj the object for which a new object pool will be created
	 * @param capacity the number of object instances to be created in the object pool
	 */
	public void CreatePoolForObject(GameObject srcObj, int capacity) {
		Queue<GameObject> objPool = null;
			
		// check if we already have a pool created for this kind of object
		if ( !poolContainer.ContainsKey(srcObj) ) {
//			Debug.Log("Created new pool of objects for object: " + srcObj.name + " -> Capacity: " + capacity);
			objPool = new Queue<GameObject>(capacity);
			poolContainer.Add(srcObj, objPool);
		} else {
//			Debug.Log("Object pool already created for object: " + srcObj.name);
			// a pool has already been created for this kind of object, just return it
			objPool = poolContainer[srcObj];
		}
			
		// extend the capacity of the pool by the specified @capacity
		for (int i = 0; i < capacity; ++i) {
			GameObject newObj = CreateInactiveInstanceOf(srcObj);
			objPool.Enqueue( newObj );
		}
	}
	
	public GameObject RequestFromPool(GameObject obj, Vector3 position, Quaternion rotation)  {
		GameObject newObj = RequestFromPool(obj, false);
		if (newObj) {
			newObj.transform.position = position;
			newObj.transform.rotation = rotation;
			newObj.SetActive(true);
		}
		
		return newObj;
	}
	
	public GameObject RequestFromPool(GameObject obj, Vector3 position)  {
		
		GameObject newObj = RequestFromPool(obj, false);
		if (newObj) {
			newObj.transform.position = position;
			newObj.SetActive(true);
		}
		
		return newObj;
	}
	
	
	/** 
	 * Same as RequestFromPool(GameObject obj, bool returnActive) method.
	 */
	public GameObject RequestFromPool(GameObject obj) {
		return RequestFromPool(obj, true);
	}
	
	/**
	 * Returns an instance of the same type as @obj specified as param
	 * @param obj the main object instance that represents the type of instance you want to request from the object pool
	 * @param returnActive a boolean mainly used for internal use
	 * @return null if the object pool doesn't exist and @autoCreatePools is false, or if the object pool is empty and @autoSizePools is false. Otherwise, if @autoCreatePools and
	 * 			 	@autoSizePools are true it will always return an instance from the specified object
	 */
	public GameObject RequestFromPool(GameObject obj, bool returnActive) {
		GameObject newObj = null;
		Queue<GameObject> objPool = poolContainer[obj];
			
		// check if the object pool requested doesn't exist
		if (objPool == null) {
			if (autoCreatePools) {
//				Debug.Log("Object pool for " + obj.name + " DOESN'T EXIST! autoCreatePools is TRUE: auto creating new object pool...");
				CreatePoolForObject(obj);
				objPool = poolContainer[obj];
			} else {
//				Debug.Log("Object pool for " + obj.name + " DOESN'T EXIST! autoCreatePools is FALSE");
				return null;
			}
		}
		
		if (objPool.Count == 0) {
			if (autoSizePools) {
				//Debug.Log("Object pool for " + obj.name + " is EMPTY! autoSizePools is TRUE: auto creating new pooled instance...");
				// create a new instance of the requested object and return it
				newObj = CreateInactiveInstanceOf(obj);
			} else {
				//Debug.Log("Object pool for " + obj.name + " is EMPTY! autoSizePools is FALSE!");
				return null;
			}
		} else {
			newObj = objPool.Dequeue();
		}

		newObj.transform.parent = null;
		if (returnActive)
			newObj.SetActive(true);
		
		//Debug.Log("Requested object from pool " + obj.name + ". Pool capacity reduced to: " + objPool.Count);
		
		return newObj;
	}
	
	/**
	 * The same as ReturnToPool(GameObject obj) but waits @delay seconds before returning the object back into its pool.
	 * @param obj the instance of the GameObject to be returned back into the pool
	 * @param delay the delay in seconds to wait before returning back the object
	 */
	public void ReturnToPool(GameObject obj, float delay) {
		StartCoroutine( DelayedReturnToPool(obj, delay) );
	}
	
	IEnumerator DelayedReturnToPool(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		
		ReturnToPool(obj);
	}
	
	/**
	 * Return an instance of a GameObject back into its pool. The user is responsable for reseting the object instance properties before
	 * returning it back to the pool. 
	 * Warning! Failing to properly reset the GameObject before returning it to its pool can create problems when re-using that object again (this usually leads to object cess-pools)
	 * You can avoid reseting you're GameObject ONLY if you are sure that you will do this when requesting it back later for re-use.
	 * If the instance of the GameObject specified as param, is not a pooled type of GameObject (doesn't contain the PoolObject component) function will just return false.
	 * @param obj the instance of the GameObject that you want to return back into its pool
	 * @return true if the object has been sucessfully returned to its pool.
	 * 			 otherwise, returns false
	 */
	public bool ReturnToPool(GameObject obj) {
		PoolObject poolObj = obj.GetComponent<PoolObject>();
		if (poolObj) {
			// re-linking the pool object back to the parent container of inactive objects belonging to the pool manager
			obj.SetActive(false);
			obj.transform.parent = poolManagerContainer.transform;
			
			//TODO: reset positions, rotations and scales to the original prefab settings, is it enough? too much auto-reseting would bring a performance penalty
			// Object reset should be manually done be each object after getting it out of the pool because that's when the object should be initialized.
			obj.transform.localPosition = poolObj.srcInstance.transform.localPosition;
			obj.transform.localRotation = poolObj.srcInstance.transform.localRotation;
			obj.transform.localScale = poolObj.srcInstance.transform.localScale;
			obj.transform.position = poolObj.srcInstance.transform.position;
			obj.transform.rotation = poolObj.srcInstance.transform.rotation;

//			if ( !poolContainer.ContainsKey(poolObj.srcInstance) ) {
//				Destroy(obj);
//				Debug.Log("WARNING!!!!! ORPHAN POOL OBJECT!! OBJECT " + poolObj.srcInstance.name + " couldn't be returned to pool. Destroying instance!");
//				return false;
//			}
			// restore the instance of the object back into its pool
			Queue<GameObject> objPool = poolContainer[poolObj.srcInstance];
			if (objPool != null) {
				objPool.Enqueue(obj);
			
				//Debug.Log("Returned object " + obj.name + " to the object pool: " + poolObj.srcInstance.name + " -> Capacity increased to: " +
				//	((Queue)poolContainer[poolObj.srcInstance]).Count);
				
				return true;
			} else {
				//Debug.Log("Tried returning object " + obj.name + " to pool, but failed! Pool doesn't exist anymore or it never did: " + poolObj.srcInstance.name);
				return false;
			}			
		}
		
		//Debug.Log("Failed to return object: " + obj.name + " to pool: " + poolObj.srcInstance.name + " -> Object doesn't contain PoolObject component");
		
		return false;
	}
	
	
//TODO:
//	public bool ReturnToPoolRecursively(GameObject obj) {
//		//TODO: return to the pool all the pooled GameObjects contained in obj starting from the base children upwards to the root of the object (GetComponentsInChildren(...) must be used)
//		return false;
//	}
//	
//	public bool ReturnToPoolRecursively(GameObject obj, float delay) {
//		//TODO: same as above but with a specified delay
//		return false;
//	}
	
	public bool DestroyObjectPool(GameObject srcObj) {
		Queue<GameObject> objPool = null;
		PoolObject poolObj = srcObj.GetComponent<PoolObject>();
		
		if (poolObj == null) {
//			Debug.Log("Removing object pool corresponding to prefab: " + srcObj.name);
			// if the src object is not a pooled object it may be the original instance object for a pool of objects
			objPool = poolContainer[srcObj];
			// remove the object pool
			poolContainer.Remove(srcObj);
		} else {
//			Debug.Log("Pooled object detected! Removing object pool corresponding to prefab: " + poolObj.srcInstance.name);
			// if it's a pooled object, obtain the object pool corresponding to this pooled object
			objPool = poolContainer[poolObj.srcInstance];
			// remove the object pool
			poolContainer.Remove(poolObj.srcInstance);
		}
		
		// if we've managed to get to the object pool, destroy all the instances from it
		if (objPool != null) {
//			Debug.Log("Removing objects from pool...");
			while(objPool.Count > 0) {
				Destroy((GameObject)objPool.Dequeue());
			}
			objPool = null;
			
			// succesfully destroyed pool
			return true;
		}
		
		// something went wrong
//		Debug.Log("WARNING! Tried to destroy object pool: " + srcObj.name + " and FAILED for some unexplained reason! :D LOOK INTO IT!");
		return false;
	}
	
	public void DestroyAllObjectPools() {
		while(poolContainer.Count > 0) {
//			Debug.Log("Object pools left to destroy: " + poolContainer.Count);
			// each time get a new enumerator for the pool container because the enumerator becomes invalid after modifying the contents of the hashtable
			IDictionaryEnumerator enumerator = poolContainer.GetEnumerator();
			enumerator.MoveNext();
			DestroyObjectPool( (GameObject)enumerator.Key );
		}
	}
	
	/**
	 * Makes a gameobject poolable and marks it as a pooled object belonging to a pool of type @prefabType
	 * @param obj the gameobject that will be converted to a pooled gameobject
	 * @param prefabType reference to a prefab object for which a pool of objects was previously created
	 * @return true if the gameobject was succesfully converted, false if something went wrong or a pool of objects for @prefabType doesn't exist
	 */
	public bool ConvertToPoolObject(GameObject obj, GameObject prefabType) {
		// check if this object isn't already a pooled object
		if (obj.GetComponent<PoolObject>() != null)
			return false;
		
		if (poolContainer[prefabType] != null)
			obj.AddComponent<PoolObject>().srcInstance = prefabType;
		else
			throw new UnityException("Trying to convert " + obj.name + " to pooled object of prefab type: " + prefabType.name + "! Pool for this prefab isn't created!");
		
		return true;
	}

	/**
	 * Ment for internal use only. Creates a new GameObject instance from the specified GameObject and makes the instance a PoolObject by adding the PoolObject component to it.
	 * @param obj the GameObject source instance
	 * @return a new GameObject instance with a PoolObject component
	 */
	private GameObject CreateInactiveInstanceOf(GameObject obj) {
		if (isBeingDestroyed) { 
			return null;
		}
		
		GameObject newInstance = (GameObject) Instantiate(obj);
		newInstance.SetActive(false);
		newInstance.transform.parent = poolManagerContainer.transform;
		
		// add the PoolObject component to make this object manageable by object pool manager
		newInstance.AddComponent<PoolObject>().srcInstance = obj;
	
		return newInstance;
	}
	
	void OnDestroy() {
		isBeingDestroyed = true;
		///TODO: investigate a weird error "Some objects were not cleaned up when closing the scene."
		Debug.Log("Destroying PoolManager");
		DestroyAllObjectPools();
		instance = null;
	}
}	

