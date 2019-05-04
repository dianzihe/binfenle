using UnityEngine;
using System.Collections;

public class ItemPresent : MonoBehaviour
{
	public GameObject presentDestroyEffect;
	
	public static event System.Action<ItemPresent> OnPresentClicked;
	
	public ItemReward rewardItem = null;
	
	private Transform destroyEffectInstance;
	
	protected UITexture uiTexture;
	
	public bool uncovered = false;
	
	public int index;
	
	void Awake()
	{
		uiTexture = GetComponent<UITexture>();
	}
	
	void OnClick()
	{
		NGUITools.PlaySound(presentDestroyEffect.GetComponent<AudioSource>().clip);
		
		DestroyPresent();	
		RaiseOnPresentClicked(this);
	}
	
	public void DestroyPresent()
	{
		destroyEffectInstance = (Instantiate(presentDestroyEffect, transform.position, Quaternion.Euler(0f, 180f, 0f)) as GameObject).transform;
		
		Vector3 cachedScale = destroyEffectInstance.localScale;
		destroyEffectInstance.parent = transform.parent;
		destroyEffectInstance.localPosition = transform.localPosition;
		destroyEffectInstance.localScale = cachedScale;
		
//		uiTexture.enabled = false;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
		
		uncovered = true;
	}
	
	public void UncoverPresent()
	{
		if (uncovered) {
			return;
		}
		
		GetComponent<Animation>().Play();
		
		rewardItem.quantityUILabel.enabled = true;
		uncovered = true;
	}
	
	protected void RaiseOnPresentClicked(ItemPresent sender)
	{
		if(OnPresentClicked != null)
		{
			OnPresentClicked(this);
		}
	}
}
