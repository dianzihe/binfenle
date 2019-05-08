using UnityEngine;
using System.Collections.Generic;

public class GameObjectsPool : MonoBehaviour
{
	//public bool forceEditorLowRes = false;
		
	private static GameObjectsPool instance;
	
	public Material[] tileBreakEffectMaterials;
	//public Material[] lowTileBreakEffectMaterials;
	
	public GameObject tileBreakEffectPrefab;
	//public GameObject lowTileBreakEffectPrefab;
		
	protected GameObject targetTileBreakEffectPrefab;
	protected Material[] targetTileBreakEffectMaterials;
		
	/// <summary>
	/// Generic pool of objects where a key can be any Unity Object. 
	/// The game knows how to search for a certain object. (for example get corresponding tile break particle effect based on material)
	/// </summary>
	public Dictionary<Object, Object> objectsPool = new Dictionary<Object, Object>(100);
	
	
	void Awake()
	{
		instance = this;
		
//#if !UNITY_EDITOR
//		forceEditorLowRes = false;
//#endif
		
		//if ( GameObjectsPool.IsLowRes || forceEditorLowRes )
		//{
		//	targetTileBreakEffectPrefab = lowTileBreakEffectPrefab;
		//	targetTileBreakEffectMaterials = lowTileBreakEffectMaterials;
		//}
		//else
		//{
			targetTileBreakEffectPrefab = tileBreakEffectPrefab;
			targetTileBreakEffectMaterials = tileBreakEffectMaterials;
		//}
		
		// Setup each type of tile break effect for each require material type and cache the particle system instances based on the material.
		for(int i = 0; i < tileBreakEffectMaterials.Length; i++)
		{
			TileBreakEffectController breakEffectInstance = (Instantiate(targetTileBreakEffectPrefab) as GameObject).GetComponent<TileBreakEffectController>();
			breakEffectInstance.UpdateTileRenderersMaterial(targetTileBreakEffectMaterials[i]);

			ParticleSystemManager.Instance.CacheParticleSystem(breakEffectInstance.gameObject, true);
			
			// Add particle system to objects pool.
			objectsPool.Add(tileBreakEffectMaterials[i], breakEffectInstance.gameObject);
		}
			
	}
	
	public static GameObjectsPool Instance {
		get {
			return instance;
		}
		set {
			instance = value;
		}
	}		
	
	public static bool IsLowRes
	{
		get 
		{
#if UNITY_IPHONE
			if (iPhone.generation == iPhoneGeneration.iPad1Gen || iPhone.generation == iPhoneGeneration.iPodTouch4Gen || iPhone.generation == iPhoneGeneration.iPhone4)
			{
				return true;
			}
#endif
			return false;
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	void OnDestroy() {
		instance = null;
	}
}
