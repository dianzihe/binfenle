using UnityEngine;
using System.Collections;

public class UnlockSystem : MonoBehaviour
{
	protected static UnlockSystem instance;
	
	public string iconName;
	public string textKey;


	public ManaItem[] unlockableitems;
	private ManaItem unlockInThisLevel;
	
	public static UnlockSystem Instance {
		get {
			return instance;
		}
	}
	
	void Awake()
	{
		foreach(ManaItem item in unlockableitems) {
			if(item.WillBeUnlocked()) {
				unlockInThisLevel = item;
				break;
			}
		}

		if(unlockInThisLevel != null) {
			instance = this;
			iconName = unlockInThisLevel.iconName;
			textKey = unlockInThisLevel.unlockTextKey;
		}else {
			Destroy(this);
		}
	}
}

