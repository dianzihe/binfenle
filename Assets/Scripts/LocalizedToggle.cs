using UnityEngine;
using System.Collections;

public class LocalizedToggle : MonoBehaviour 
{
	public string nameKey;
	public string onKey;
	public string offKey;
	
	public string defaultName = "Toggle: ";
	public string defaultOn = "ON";
	public string defaultOff = "OFF";
	
	protected UILabel label;
	
	void Awake()
	{
		label = GetComponent<UILabel>();
	}
	
	public void UpdateValue(int on)
	{
		if (label == null) {
//			Debug.LogWarning("No lable found on toggle: " + name);
			return;
		}
		/* 
		if (nameKey != null && nameKey != "") {
			label.text = Language.Get(nameKey);
		} else {
			label.text = defaultName;
		}
		
		if (on == 0) {
			if (offKey != null && offKey != "") {
				label.text += " " + Language.Get(offKey);
			} else {
				label.text += ": " + defaultOff;
			}
		} else {
			if (onKey != null && onKey != "") {
				label.text += " " + Language.Get(onKey);
			} else {
				label.text += ": " + defaultOn;
			}
		}
		*/
	}
}
