using UnityEngine;
using System.Collections;

public class LocalizedPosition : MonoBehaviour 
{
	public Vector3 defaultPosition = Vector3.zero;
	public LangPos[] customPositions;
	
	void Start () 
	{
		transform.localPosition = defaultPosition;
		
		if (customPositions != null && customPositions.Length > 0) {
			string lang = Language.CurrentLanguage().ToString().ToLower();
			
			for (int i = 0; i < customPositions.Length; ++i)
			{
				if (lang == customPositions[i].lang) {
					transform.localPosition = customPositions[i].localPos;
					break;
				}
			}
		}
	}
}

[System.Serializable]
public class LangPos {
	public string lang;
	public Vector3 localPos;
}