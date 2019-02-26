using UnityEngine;
using System.Collections;

public class WinterchillEffect : DestroyEffect
{
	public bool waitBetweenTiles = false;
	
	[System.NonSerialized]
	//public TilesTriggerListener triggerListener;
			
	//[System.NonSerialized]
	public BoxCollider freezeTrigger;
	
	[System.NonSerialized]
	public Transform triggerTransform;
	
	/// <summary>
	/// The number tile layers affected. By tile layer means a row or column of tiles affected (depending if the effect is horizontal or vertical).
	/// </summary>
	public float numTileLayersAffected = 1f;
	
	[System.NonSerialized]
	public float scaleSpeed = 15.5f;
	
	private bool stopTriggerScale = false;


	protected override void Awake () {
		base.Awake ();
		
		triggerTransform = cachedTransform.Find("FreezeTrigger");
		freezeTrigger = triggerTransform.GetComponent<Collider>() as BoxCollider;
		//triggerListener = triggerTransform.GetComponent<TilesTriggerListener>();
	}
	
	public override void InitComponent () {
		base.InitComponent ();
		stopTriggerScale = false;
	}

	public void Launch(DirectionalDestroyTile sender, float percent, float durationPercent, float totalLineSize) {
		triggerTransform.localScale = new Vector3((0.75f + numTileLayersAffected) * Match3BoardRenderer.horizTileDistance, 1f, 0f);
	
		GetComponent<ParticleSystem>().startLifetime *= percent;
		StartCoroutine(ScaleFreezeTrigger());
		StartCoroutine(StopParticles(GetComponent<ParticleSystem>().duration * durationPercent));
		
		gameObject.SetActive(true);	
	}
	
	IEnumerator StopParticles(float stopTime)
	{
		yield return new WaitForSeconds(stopTime);
		GetComponent<ParticleSystem>().Stop();
		
		// Disable freeze trigger
//		freezeTrigger.enabled = false;
		stopTriggerScale = true;
		
		if (OnEffectFinished != null) {
			OnEffectFinished(this);
		}
	}
	
	IEnumerator ScaleFreezeTrigger() {
		while(!stopTriggerScale) {
			// Scale the trigger
			triggerTransform.localScale += Vector3.forward * scaleSpeed * Time.deltaTime;

			// Reposition the trigger so the collider scales in the local forward direction only.
			Vector3 newLocalPos = triggerTransform.localPosition;
			newLocalPos.x = 0f;
			//TODO: offset hack on local Y axis of the trigger because it's offseted already with the same amount on the world Z axis in "DirectionalDestroyTile"
			newLocalPos.y = -19f;
			newLocalPos.z =	triggerTransform.localScale.z * 0.5f;
			
			triggerTransform.localPosition = newLocalPos;

			yield return null;
		}
	}
}

