using UnityEngine;
using System.Collections;

public class ItemReward : MonoBehaviour
{
	
	public enum RewardType
	{
		Heart,
		Hourglass,
		Icepick,
		Powerup,
		Snowball,
	};
	
	public RewardType rewardType;
	
	public GameObject selectionEffect;
	
	protected GameObject selectionEffectInstance;
	protected Vector3 selectedEffectOffset = Vector3.forward * -10f;
	
	public UILabel quantityUILabel;
	
	public int quantity = 1;
	
	protected Transform cachedTransform;
	
	protected void Awake()
	{
		cachedTransform = transform;
		UpdateQuantitylabel();
		quantityUILabel.enabled = false;
	}
	
	public void ShowSelection()
	{
		if(selectionEffectInstance)
		{
			selectionEffectInstance.SetActive(true);
		}
		else
		{
			selectionEffectInstance = Instantiate(selectionEffect) as GameObject;
			Vector3 myScale = selectionEffectInstance.transform.localScale;
			selectionEffectInstance.transform.parent = cachedTransform;
			selectionEffectInstance.transform.localPosition = Vector3.zero + selectedEffectOffset;
			selectionEffectInstance.transform.localScale = myScale;
		}
	}
	
	public void LightBackground()
	{
		quantityUILabel.enabled = true;
		cachedTransform.Find("Background").GetComponent<UISprite>().spriteName = "gui_powerup_bgon";
	}
	
	public void HideSelection()
	{
		if(selectionEffectInstance)
		{
			selectionEffectInstance.SetActive(false);
		}
	}
	
	protected void UpdateQuantitylabel()
	{
		quantityUILabel.text = quantity.ToString();
	}
	
	public void AwardItem()
	{
		switch (rewardType)
		{
		case RewardType.Heart:
			LivesSystem.instance.Lives += quantity;
			LivesSystem.SaveLivesAndNotify(LivesSystem.instance.Lives, long.Parse(PlayerPrefs.GetString(LivesSystem.livesTimeKey, LivesSystem.TimeSeconds().ToString())));
			PlayerPrefs.Save();
			break;
		case RewardType.Powerup:
			TokensSystem.Instance.itemTokens += quantity;
			TokensSystem.Instance.SaveItems();
			break;
		case RewardType.Icepick:
			TokensSystem.Instance.icePicks += quantity;
			TokensSystem.Instance.SaveItems();
			break;
		case RewardType.Snowball:
			TokensSystem.Instance.snowballs += quantity;
			TokensSystem.Instance.SaveItems();
			break;
		case RewardType.Hourglass:
			TokensSystem.Instance.hourglasses += quantity;
			TokensSystem.Instance.SaveItems();
			break;
		default:
			break;
		}
	}
}
