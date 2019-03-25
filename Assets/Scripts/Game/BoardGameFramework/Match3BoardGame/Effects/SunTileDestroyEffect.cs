using UnityEngine;
using System.Collections;

public class SunTileDestroyEffect : MonoBehaviour
{
	public TilesTriggerListener triggerListener;
	protected Transform cachedTransform;
	
	public float scaleTarget;
	public float animationTime;
	
	void Awake()
	{
		cachedTransform = transform;
	}
			
	protected IEnumerator ColliderScaler()
	{
		float lerpAmount = 0f;
		
		while(lerpAmount <= 1)
		{	
			float tileSize = Match3BoardRenderer.Instance.horizontalTileDistance;
			Vector3 initialScale = Vector3.forward * tileSize;
			
			cachedTransform.localScale = initialScale + Vector3.right * scaleTarget * lerpAmount + Vector3.up * scaleTarget * lerpAmount;
			
			if(lerpAmount == 1f)
			{
				yield break;
			}
			
			lerpAmount = Mathf.Clamp01(lerpAmount + 1.0f/animationTime * Time.deltaTime);
			yield return null;
		}
	}
	
	public void StartColliderScaleEfect()
	{
		StartCoroutine(ColliderScaler());
	}
}
