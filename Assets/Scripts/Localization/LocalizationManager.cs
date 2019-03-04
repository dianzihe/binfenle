using UnityEngine;
using System.Collections;

public class LocalizationManager {
	
	public static string currentLanguage;
	private static Hashtable textTable;
	
	public static string resourceExtension = "_en";
	
	private static string customizedPath = "Story/"; //Customized path for localization on Resources folder
	
	/// <summary>
	/// Load a new language translation inside a Resources folder with the name "SystemLanguage" and type "TextAsset". Filename should have .txt extension
	/// </summary>
	/// <param name="language">
	/// The language to load <see cref="SystemLanguage"/>
	/// </param>
	/// <returns>
	/// Returns true if the language were correctly loaded, otherwise returns false <see cref="System.Boolean"/>
	/// </returns>
	public static bool LoadLanguage(SystemLanguage language)
	{
		string newLanguage = System.Enum.GetName(typeof(SystemLanguage), language);
		//Debug.Log("language = " + newLanguage + " old: " + currentLanguage);
		if(currentLanguage != newLanguage || !hasLanguageLoaded())
		{
			currentLanguage = newLanguage;
			
			if (language == SystemLanguage.English) {
				resourceExtension = "_en";
			} else if (language == SystemLanguage.German) {
				resourceExtension = "_de";
			} else if (language == SystemLanguage.Spanish) {
				resourceExtension = "_es";
			} else if (language == SystemLanguage.Swedish) {
				resourceExtension = "_se";
			} else if (language == SystemLanguage.Italian) {
				resourceExtension = "_it";
			} else if (language == SystemLanguage.French) {
				resourceExtension = "_fr";
			} else if (language == SystemLanguage.Dutch) {
				resourceExtension = "_nl";
			} else if (language == SystemLanguage.Chinese) {
				resourceExtension = "_cn";	
			} else if (language == SystemLanguage.Japanese) {
				resourceExtension = "_jp";	
			} else if (language == SystemLanguage.Korean) {
				resourceExtension = "_kr";	
			} else if (language == SystemLanguage.Russian) {
				resourceExtension = "_ru";	
			}
			
			// if we want to store the language files with another path structure inside a  ".../Resources/" folder
			string fullpath = customizedPath+"Text/"+currentLanguage;
	
			TextAsset textAsset = (TextAsset) Resources.Load(fullpath, typeof(TextAsset));
			if (textAsset == null) 
			{
				//Debug.Log(fullpath + " file not found. Language not loaded");
				return false;
			}
			
			// create the text hash table if one doesn't exist
			if (textTable == null) 
			{
				textTable = new Hashtable();
			}
				
			// clear the hashtable
			textTable.Clear();
			
			System.IO.StringReader reader = new System.IO.StringReader(textAsset.text);
			string key;
			string val;
			while(true)
			{
	    		key = reader.ReadLine();
	    		val = reader.ReadLine();
	    		if (key != null && val != null) 
	    		{
					if(textTable.ContainsKey(key))
					{
						//Debug.Log("Duplicated key '"+key+"' on language file: "+fullpath);
					}
					else
					{
						//Debug.Log(val);
						if (val.Contains("\\n")) {
							val = val.Replace("\\n", System.Environment.NewLine);
						}
	    				textTable.Add(key, val);
					}
	    		} 
	    		else 
	    		{
	    			break;
	    		}
			}
	
			reader.Close();
			
			return true;
		}
				
		return false;
	}
	
	/// <summary>
	/// Check if the manager has loaded a languaje
	/// </summary>
	/// <returns>
	/// Return true if the manager contais a language
	/// </returns>
	public static bool hasLanguageLoaded()
	{
		if(Language.CurrentLanguage() != LanguageCode.N) {
			return true;
		}
		
//		if(textTable != null)
//		{
//			if(textTable.Count>0)
//			{
//				return  true;
//			}
//		}
		
		return false;
	}
	
	/// <summary>
	/// Get the current language loaded on manager
	/// </summary>
	/// <returns>
	/// Return the language name <see cref="System.String"/>
	/// </returns>
	public static string getLanguageLoaded()
	{
		return currentLanguage;
	}
	
	/// <summary>
	/// Return the associated translation on the loaded language
	/// </summary>
	/// <param name="key">
	/// Key associated with the translation <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// Return the translated string associated <see cref="System.String"/>
	/// </returns>
	public static string GetText(string key)
	{
		if(textTable != null)
		{
			if(textTable.ContainsKey(key))
			{
				return  (string)textTable[key];
			}
			else
			{
				return key;
			}
		}
		
		return "";
	}
	
	/// <summary>
	/// Return the number of lines of the associated translation on the loaded language
	/// </summary>
	/// <param name="key">
	/// Key associated with the translation <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// Return the corresponding int of text lines
	/// </returns>
	public static int GetTextLines(string key)
	{
		if(textTable != null)
		{
			if(textTable.ContainsKey(key))
			{
				string[] splittedText = ((string)textTable[key]).Split('|');
				return  splittedText.Length;
			}
			else
			{
				return 0;
			}
		}
		
		return 0;
	}
	
	/// <summary>
	/// Return the associated translated asset on the loaded language. For performance issues, is recommended to cached this value
	/// </summary>
	/// <param name="key">
	/// Key associated with the asset <see cref="object"/>
	/// </param>
	/// <returns>
	/// Return the translated asset associated <see cref="System.String"/>
	/// </returns>
	public static Object GetAsset(string key)
	{
		// TODO: Handle exception if asset is not found or language is not selected
		Object asset = Resources.Load(currentLanguage+"/"+key);
		if(asset == null)
		{
			asset = Resources.Load("default/AssetNotFound");
		}
		
		return asset;
	}
	
	public static Object GetLocution(string key)
	{
		return GetLocution(key, customizedPath);
	}
	
	public static Object GetLocution(string key, string path)
	{
//		Debug.Log(path + "Sounds/Voices/" + currentLanguage + "/" + key);
		// TODO: Handle exception if asset is not found or language is not selected
		Object asset = Resources.Load(path + "Sounds/Voices/" + currentLanguage + "/" + key);
		if(asset == null)
		{
			asset = Resources.Load(customizedPath + "Sounds/Voices/default/AssetNotFound");
		}
		
		return asset;
	}
}
