using UnityEngine;
using System.Collections;

public class UnlockLabel : MonoBehaviour
{
	public UISprite icon;
	
	protected UILabel myLabel;
	protected GameObject daddy;
	protected bool available = false;
	
	public bool Available
	{
		get {
			return available;
		}
	}
	
	void Start()
	{
		UnlockSystem unlockSystem = UnlockSystem.Instance;
		if (unlockSystem == null) {
			Debug.LogWarning("No unlock system");
			return;
		}
		
		available = true;
		
		myLabel = GetComponent<UILabel>();
		myLabel.text = Language.Get(unlockSystem.textKey);
		
		icon.spriteName = unlockSystem.iconName;
	}
}

