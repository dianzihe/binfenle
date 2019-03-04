using UnityEngine;
using System.Collections;

public class LocalizableImage : MonoBehaviour 
{
	public string path;
	
	public CountryLanguage[] specificCountries;
	
	// Use this for initialization
	void Start () 
	{
		string lang = Language.CurrentLanguage().ToString().ToLower();
		
		if (specificCountries != null && specificCountries.Length > 0) 
		{
			string currentCountry = "US";
			
			foreach (CountryLanguage entry in specificCountries) 
			{
				if (entry.country == currentCountry && lang == entry.language) 
				{
					lang = entry.useLanguage;
					break;
				}
			}
		}
		
		Texture newTexture = Resources.Load(path + lang) as Texture;
		
		if (newTexture == null) {
			newTexture = Resources.Load(path + "en") as Texture;
		}
		
		GetComponent<Renderer>().material.mainTexture = newTexture;
	}
}

[System.Serializable]
public class CountryLanguage
{
	public string country;
	public string language;
	public string useLanguage;
}