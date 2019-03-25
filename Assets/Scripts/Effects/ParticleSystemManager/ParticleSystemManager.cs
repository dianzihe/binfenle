/// <summary>
/// Particle system manager.
/// Notes: 
/// 	-The particle gameObject will use the rotation of the prefab
/// 	-The maximum number of particles is set on the particle system
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleSystemManager : MonoBehaviour {
	private static ParticleSystemManager instance = null;

	public class ParticleCacheData
	{
		public GameObject				instantiatedGameObject;
		public ParticleSystem[]			cachedParticleSystem;
		public ParticleBurstQuantity[]	burstQuantity;
		public Transform				cachedTransform;
		
		public ParticleCacheData(GameObject _particleSystemGameObject)
		{
			//Cache variables
			instantiatedGameObject 	=	_particleSystemGameObject;
			cachedTransform			= 	instantiatedGameObject.transform;
			cachedParticleSystem	= 	instantiatedGameObject.GetComponentsInChildren<ParticleSystem>();
			burstQuantity			= 	instantiatedGameObject.GetComponentsInChildren<ParticleBurstQuantity>();
			
			if (cachedParticleSystem.Length != burstQuantity.Length)
				Debug.LogWarning("You have forgot to add the ParticleBurstQuantity sript on the particleSystem: " + _particleSystemGameObject.name +" you should only use Emit");
		}
	}
	
	private Dictionary<GameObject,ParticleCacheData> particleObjectReferences = new Dictionary<GameObject, ParticleCacheData>();
	
	
	void Awake() 
	{
		instance = this;
	}
	
	public static ParticleSystemManager Instance {
		get {
			if (instance == null) 
			{
				GameObject particleSystemMgr = new GameObject("_ParticleSystemManager");
				instance = particleSystemMgr.AddComponent<ParticleSystemManager>();
			}	

			return instance;
		}
	}	

	/// <summary>
	/// Caches the particle system.
	/// </summary>
	public GameObject CacheParticleSystem(GameObject _particleGameObject, bool alreadyInstantiated = false)
	{
		//Check if its already on the dictionary
		if (!particleObjectReferences.ContainsKey(_particleGameObject))
		{
			GameObject tempInstantiatedParticleObject;
			if ( !alreadyInstantiated ) {
				tempInstantiatedParticleObject = Instantiate(_particleGameObject) as GameObject;
			} else {
				tempInstantiatedParticleObject = _particleGameObject;
			}
			
			ParticleCacheData tempParticleCacheData = new ParticleCacheData(tempInstantiatedParticleObject);
			particleObjectReferences.Add(_particleGameObject,  tempParticleCacheData);
			tempInstantiatedParticleObject.SetActive(false); //Disabled until you need it
			
			return tempInstantiatedParticleObject;
		}
		
		return null;
	}

	
	/// <summary>
	/// Play the specified _particleGameObject in _positio. And set if autoCache it
	/// </summary>

	public void Play(GameObject _particleGameObject,Vector3 _position, bool _autoCache)
	{
		ParticleCacheData particleSystemToUse = ConfigureParticleSystemForPlay(_particleGameObject, _position,_autoCache);
		
		if (particleSystemToUse != null)
			StartCoroutine(DelayedParticlePlay(particleSystemToUse.cachedParticleSystem[0]));
		
//		else
//			Debug.LogError(string.Format("You are trying to emit from {0} without having caching it before" + _particleGameObject.name));
	
	}
	
	/// <summary>
	/// Play the specified _particleGameObject in _positio. And set if autoCache it
	/// </summary>
	public void Emit(GameObject _particleGameObject,Vector3 _position, bool _autoCache)
	{
		ParticleCacheData particleSystemToUse = ConfigureParticleSystemForPlay(_particleGameObject, _position, _autoCache);
		
		if (particleSystemToUse.burstQuantity.Length != particleSystemToUse.cachedParticleSystem.Length) //Protection to use Emit in a not ready prefab
			Play(_particleGameObject,_position,_autoCache);
		
		
		if (particleSystemToUse != null)
		{
			for (int i = 0;  i< particleSystemToUse.cachedParticleSystem.Length; i++)
			{
				particleSystemToUse.cachedParticleSystem[i].Emit(particleSystemToUse.burstQuantity[i].burstQuantity);	
			}
		}
//		else
//			Debug.LogError(string.Format("You are trying to emit from {0} without having caching it before" + _particleGameObject.name));
	
	}
	
	
	/// <summary>
	/// Configures the particle system for play.
	/// </summary>

	private ParticleCacheData ConfigureParticleSystemForPlay(GameObject _particleGameObject,Vector3 _position, bool _autoCache)
	{
		if (_autoCache)
			CacheParticleSystem(_particleGameObject);
		
		ParticleCacheData particleSystemToUse = particleObjectReferences[_particleGameObject];
		
		if (particleSystemToUse != null)
		{
			particleSystemToUse.cachedTransform.position = _position;
			particleSystemToUse.instantiatedGameObject.SetActive(true);
			return particleSystemToUse;
		}
		return null;
	}
	
	
	//Worarround for the Play() bug
	IEnumerator DelayedParticlePlay(ParticleSystem _particleSystemToPlay)
	{
		yield return new WaitForEndOfFrame();
		_particleSystemToPlay.Play();
	}
	
	
}
