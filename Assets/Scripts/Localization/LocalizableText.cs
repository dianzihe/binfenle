using UnityEngine;
using System.Collections;
using System;

public class LocalizableText : MonoBehaviour {
	
	public string Asian_id = "";
	public string keyText;
	public int indexOfMultilineText = 0;
	//[System.NonSerialized]
	public static float ChineseCharSize = 1;
	public static float ChineseCharSizeModifier = 5;
	public bool deactivateOnNotFound = true;
	// Use this for initialization
	void Start () {
		updateText();
	}
	
	void OnEnable() {
		updateText();
	}
	
	public void updateText() {
		updateText(5);
	}
	
	public static bool isLanguageAsian(string language) {
		if(language == "Chinese" || language == "Korean" || language == "Japanese") return true;
		// || language == "Russian"
		else return false;
	}
	
	public static string getAsianFontPath(string language, bool hd) {
		string result = "Menu/Fonts/" + "story_" + language.ToLower() + (hd ? "_hd" : "_sd");
	    return result;
		
	}
	
	public string getAsianFontPath_with_id(string language, bool hd) {
		//string result = "Menu/Fonts/" + "story_" + language.ToLower() + (hd ? "_hd" : "_sd");
		string result = "Menu/Fonts/" + "story_" + language.ToLower() + Asian_id + (hd ? "_hd" : "_sd");
		return result;
		
	}
	
	public void updateText(int multilineIndexVisible) {
		
		//Support NGUI Label
		UILabel uiLabel = GetComponent<UILabel>();

		if(uiLabel) {
			uiLabel.text = Language.Get(keyText).Replace("<SPACE>", " ");
			return;
		}

		if(LocalizationManager.hasLanguageLoaded())
		{
			string localizedText = Language.Get(keyText).Replace("<SPACE>", " ");
			
			if (localizedText.Contains("|")) {
				string[] splittedText = localizedText.Split('|');
				if(indexOfMultilineText < splittedText.Length)
				{
					TextMesh mesh = GetComponent<TextMesh>();
					bool same = mesh.text == splittedText[indexOfMultilineText];
					if (!same) {
						mesh.text = splittedText[indexOfMultilineText];
					}
					if (indexOfMultilineText > multilineIndexVisible) {
						gameObject.SetActive(false);
					} else {
						gameObject.SetActive(true);
					}
				}
				else
				{
					if(deactivateOnNotFound)
						gameObject.SetActive(false);
					//Debug.Log("ERROR ON LOCALIZABLE TEXT: index for Multiline text is higher than text lines in localization file, for language "+LocalizationManager.getLanguageLoaded());
				}
			}
			else if (indexOfMultilineText == 0) {
				TextMesh mesh = GetComponent<TextMesh>();
				bool same = mesh.text == localizedText;
				if (!same) {
					mesh.text = localizedText;
				}
				gameObject.SetActive(true);
			} 
			else if(deactivateOnNotFound) {
				gameObject.SetActive(false);
			}
		}
		else
		{
			if(deactivateOnNotFound) {
				//gameObject.SetActive(false;
				TextMesh mesh = GetComponent<TextMesh>();
				mesh.text = "";
			}
			//Debug.Log("ERROR ON LOCALIZABLE TEXT: text '"+keyText+"' couldn't be find. Localization manager has no Language loaded");
			
		}
		
	}
}
