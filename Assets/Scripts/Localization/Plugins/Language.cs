// Localization pacakge by Mike Hergaarden - M2H.nl
// DOCUMENTATION: http://www.m2h.nl/files/LocalizationPackage.pdf
// Thank you for buying this package!

//Version 1.01 - 04-07-2011

using UnityEngine;
using System.Collections;
using System.Globalization;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class Language
{

    //For settings, see TOOLS->LOCALIZATION
    
    public static string settingsAssetPath = "Assets/Localization/Resources/Languages/LocalizationSettings.asset";
    public static LocalizationSettings settings = (LocalizationSettings)Resources.Load("Languages/" + System.IO.Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));

    //Privates
    static List<string> availableLanguages;
    static LanguageCode currentLanguage = LanguageCode.N;


    static Dictionary<string, Hashtable> currentEntrySheets;
	static Dictionary<string, Hashtable> defaultEntrySheets;

    static Language()
    {
        //if(settings == null)
        //    settings = (LocalizationSettings)Resources.Load("Languages/" + System.IO.Path.GetFileNameWithoutExtension(settingsAssetPath), typeof(LocalizationSettings));
        LoadAvailableLanguages();

        bool useSystemLanguagePerDefault = settings.useSystemLanguagePerDefault;
        LanguageCode useLang = LocalizationSettings.GetLanguageEnum(settings.defaultLangCode);    //ISO 639-1 (two characters). See: http://en.wikipedia.org/wiki/List_of_ISO_639-1_codes

        //See if we can use the local system language: if so, we overwrite useLang
        if (useSystemLanguagePerDefault)
        {
            //Use Unity system lang
            LanguageCode localLang = LanguageNameToCode(Application.systemLanguage);
			//TODO: use the hack below to override current language (comment the line for release builds)
//			localLang = LanguageCode.JA;
			
            //Otherwise try .NET cultureinfo; doesnt work on mobile systems
            if (localLang == LanguageCode.N)
            {
                string langISO = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                if(langISO!="iv") //IV = InvariantCulture
                    localLang = LocalizationSettings.GetLanguageEnum(langISO);
            }
            if (availableLanguages.Contains(localLang + ""))
                useLang = localLang;
        }

        //See if we can use the last used language (playerprefs)
        string lastLang = PlayerPrefs.GetString("M2H_lastLanguage", "");

        if (lastLang != "" && availableLanguages.Contains(lastLang))
        {
            SwitchLanguage(lastLang);
        }
        else
        {
            SwitchLanguage(useLang);
        }

    }



    static void LoadAvailableLanguages()
    {
        availableLanguages = new List<string>();
        Debug.Log(settings);
        if (settings.sheetTitles == null || settings.sheetTitles.Length <= 0)
        {
            Debug.Log("None available");
            return;

        }
        foreach (LanguageCode item in Enum.GetValues(typeof(LanguageCode)))
        {
            if (HasLanguageFile(item + "", settings.sheetTitles[0]))
            {
                availableLanguages.Add(item + "");
            }
        }
        Resources.UnloadUnusedAssets();//Clear all loaded language files
    }

   

    public static string[] GetLanguages()
    {
        return availableLanguages.ToArray();
    }

    public static bool SwitchLanguage(string langCode)
    {
        return SwitchLanguage(LocalizationSettings.GetLanguageEnum(langCode));
    }
	
    public static bool SwitchLanguage(LanguageCode code)
    {
        if (availableLanguages.Contains(code + ""))
        {
            DoSwitch(code);
            return true;
        }
        else
        {
            Debug.LogError("Could not switch from language " + currentLanguage + " to " + code);
            if (currentLanguage == LanguageCode.N)
            {
                if (availableLanguages.Count > 0)
                {
                    DoSwitch(LocalizationSettings.GetLanguageEnum(availableLanguages[0]));
                    Debug.LogError("Switched to " + currentLanguage + " instead");
                }
                else
                {
                    Debug.LogError("Please verify that you have the file: Resources/Languages/" + code + "");
                    Debug.Break();
                }
            }

            return false;
        }

    }

    static void DoSwitch(LanguageCode newLang)
    {
		Debug.Log("Switching language: " + newLang);
        PlayerPrefs.GetString("M2H_lastLanguage", newLang + "");

        currentLanguage = newLang;
        currentEntrySheets = new Dictionary<string, Hashtable>();
		defaultEntrySheets = new Dictionary<string, Hashtable>();

        XMLParser xmlparser = new XMLParser();
		
		//load language from server, if possible
        foreach (string sheetTitle in settings.sheetTitles)
        {
            currentEntrySheets[sheetTitle] = new Hashtable();

            Hashtable main = (Hashtable)xmlparser.Parse(GetLanguageFileContents(sheetTitle));
            ArrayList entries = (ArrayList)(((ArrayList)main["entries"])[0] as Hashtable)["entry"];
            foreach (Hashtable entry in entries)
            {
                string tag = (string)entry["@name"];
                string data = (entry["_text"] + "").Trim();
                data = data.UnescapeXML();
                (currentEntrySheets[sheetTitle])[tag] = data;
            }
        }
		
		//load default language file
		foreach (string sheetTitle in settings.sheetTitles)
        {
			if (HasLanguageFileLocally(currentLanguage + "", sheetTitle)) {
	            defaultEntrySheets[sheetTitle] = new Hashtable();
	
	            Hashtable main = (Hashtable)xmlparser.Parse(((TextAsset)Resources.Load("Languages/" + currentLanguage + "_" + sheetTitle, typeof(TextAsset))).text);
	            ArrayList entries = (ArrayList)(((ArrayList)main["entries"])[0] as Hashtable)["entry"];
	            foreach (Hashtable entry in entries)
	            {
	                string tag = (string)entry["@name"];
	                string data = (entry["_text"] + "").Trim();
	                data = data.UnescapeXML();
	                (defaultEntrySheets[sheetTitle])[tag] = data;
	            }
			}
			else {
				defaultEntrySheets = currentEntrySheets;
			}
        }
		
        //Update all localized assets
        LocalizedAsset[] assets = (LocalizedAsset[])GameObject.FindObjectsOfType(typeof(LocalizedAsset));
        foreach (LocalizedAsset asset in assets)
        {
            asset.LocalizeAsset();
        }

        SendMonoMessage("ChangedLanguage", currentLanguage);
    }

    //Get a localized asset for the current language
    static public UnityEngine.Object GetAsset(string name)
    {
        return Resources.Load("Languages/Assets/" + CurrentLanguage() + "/" + name);
    }

    //Lang files
    static bool HasLanguageFile(string lang, string sheetTitle)
    {
        return ((TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null) 
        /* || LocalizationServerManager.Instance.HasLanguageFile(lang, sheetTitle)*/;
    }
	
	static bool HasLanguageFileLocally(string lang, string sheetTitle)
    {
        return ((TextAsset)Resources.Load("Languages/" + lang + "_" + sheetTitle, typeof(TextAsset)) != null);
    }

    static string GetLanguageFileContents(string sheetTitle)
    {
        /* 
		if (LocalizationServerManager.Instance.HasLanguageFile(currentLanguage.ToString(), sheetTitle))
		{
			return LocalizationServerManager.Instance.GetLanguageFileContents(currentLanguage.ToString(), sheetTitle);
		}
		else
		{
            */
	        TextAsset ta = (TextAsset)Resources.Load("Languages/" + currentLanguage + "_" + sheetTitle, typeof(TextAsset));
	        return ta.text;
		//}
    }


    public static LanguageCode CurrentLanguage()
    {
        return currentLanguage;
    }

    public static string Get(string key)
    {
        return Get(key, settings.sheetTitles[0]);
    }


    public static string Get(string key, string sheetTitle)
    {
        if (currentEntrySheets == null || !currentEntrySheets.ContainsKey(sheetTitle))
        {
            Debug.LogError("The sheet with title \""+sheetTitle+"\" does not exist!");
            return "";
        }
        if ((currentEntrySheets[sheetTitle]).ContainsKey(key))
        {
            return ((string)(currentEntrySheets[sheetTitle])[key]).Replace("|", "\n");
        }
		else if ((defaultEntrySheets[sheetTitle]).ContainsKey(key))
        {
            return ((string)(defaultEntrySheets[sheetTitle])[key]).Replace("|", "\n");
        }
        else
        {
          //  Debug.LogError("MISSING LANG:" + key);
            return "MISSING LANG:" + key;
        }
    }
	
	public static string Get(string key, params object[] paramList) {
		string ret = Get (key);
		
		if(paramList == null) {
			return ret;
		}
		
		int index = 0;
		string[] retSplit = Regex.Split(ret, @"{\d}");
		
		ret = string.Empty;
		
		for(int i = 0; i < retSplit.Length; i++) {
			
			Debug.Log("Attempting with: i= " + i + " index =" + index + retSplit.Length);
			ret += retSplit[i];
			
			if(index < retSplit.Length-1 && paramList[index]!=null)  {
				ret += paramList[index];
				index++;
			}
		}
		
		return ret;
	}

    static void SendMonoMessage(string methodString, params object[] parameters)
    {
        if (parameters != null && parameters.Length > 1) Debug.LogError("We cannot pass more than one argument currently!");
        GameObject[] gos = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in gos)
        {
            if (go && go.transform.parent == null)
            {
                if (parameters != null && parameters.Length == 1)
                {
                    go.gameObject.BroadcastMessage(methodString, parameters[0], SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    go.gameObject.BroadcastMessage(methodString, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    public static LanguageCode LanguageNameToCode(SystemLanguage name)
    {
        if (name == SystemLanguage.Afrikaans) return LanguageCode.AF;
        else if (name == SystemLanguage.Arabic) return LanguageCode.AR;
        else if (name == SystemLanguage.Basque) return LanguageCode.BA;
        else if (name == SystemLanguage.Belarusian) return LanguageCode.BE;
        else if (name == SystemLanguage.Bulgarian) return LanguageCode.BG;
        else if (name == SystemLanguage.Catalan) return LanguageCode.CA;
        else if (name == SystemLanguage.Chinese) return LanguageCode.ZH;
        else if (name == SystemLanguage.Czech) return LanguageCode.CS;
        else if (name == SystemLanguage.Danish) return LanguageCode.DA;
        else if (name == SystemLanguage.Dutch) return LanguageCode.NL;
        else if (name == SystemLanguage.English) return LanguageCode.EN;
        else if (name == SystemLanguage.Estonian) return LanguageCode.ET;
        else if (name == SystemLanguage.Faroese) return LanguageCode.FA;
        else if (name == SystemLanguage.Finnish) return LanguageCode.FI;
        else if (name == SystemLanguage.French) return LanguageCode.FR;
        else if (name == SystemLanguage.German) return LanguageCode.DE;
        else if (name == SystemLanguage.Greek) return LanguageCode.EL;
        else if (name == SystemLanguage.Hebrew) return LanguageCode.HE;
        else if (name == SystemLanguage.Hungarian) return LanguageCode.HU;
        else if (name == SystemLanguage.Icelandic) return LanguageCode.IS;
        else if (name == SystemLanguage.Indonesian) return LanguageCode.ID;
        else if (name == SystemLanguage.Italian) return LanguageCode.IT;
        else if (name == SystemLanguage.Japanese) return LanguageCode.JA;
        else if (name == SystemLanguage.Korean) return LanguageCode.KO;
        else if (name == SystemLanguage.Latvian) return LanguageCode.LA;
        else if (name == SystemLanguage.Lithuanian) return LanguageCode.LT;
        else if (name == SystemLanguage.Norwegian) return LanguageCode.NO;
        else if (name == SystemLanguage.Polish) return LanguageCode.PL;
        else if (name == SystemLanguage.Portuguese) return LanguageCode.PT;
        else if (name == SystemLanguage.Romanian) return LanguageCode.RO;
        else if (name == SystemLanguage.Russian) return LanguageCode.RU;
        else if (name == SystemLanguage.SerboCroatian) return LanguageCode.SH;
        else if (name == SystemLanguage.Slovak) return LanguageCode.SK;
        else if (name == SystemLanguage.Slovenian) return LanguageCode.SL;
        else if (name == SystemLanguage.Spanish) return LanguageCode.ES;
        else if (name == SystemLanguage.Swedish) return LanguageCode.SW;
        else if (name == SystemLanguage.Thai) return LanguageCode.TH;
        else if (name == SystemLanguage.Turkish) return LanguageCode.TR;
        else if (name == SystemLanguage.Ukrainian) return LanguageCode.UK;
        else if (name == SystemLanguage.Vietnamese) return LanguageCode.VI;
        else if (name == SystemLanguage.Hungarian) return LanguageCode.HU;
        else if (name == SystemLanguage.Unknown) return LanguageCode.N;
        return LanguageCode.N;
    }

}

#region enums



public enum LanguageCode
{
    N,//null
    AA, //Afar
    AB, //Abkhazian
    AF, //Afrikaans
    AM, //Amharic
    AR, //Arabic
    AS, //Assamese
    AY, //Aymara
    AZ, //Azerbaijani
    BA, //Bashkir
    BE, //Byelorussian
    BG, //Bulgarian
    BH, //Bihari
    BI, //Bislama
    BN, //Bengali
    BO, //Tibetan
    BR, //Breton
    CA, //Catalan
    CO, //Corsican
    CS, //Czech
    CY, //Welch
    DA, //Danish
    DE, //German
    DZ, //Bhutani
    EL, //Greek
    EN, //English
    EO, //Esperanto
    ES, //Spanish
    ET, //Estonian
    EU, //Basque
    FA, //Persian
    FI, //Finnish
    FJ, //Fiji
    FO, //Faeroese
    FR, //French
    FY, //Frisian
    GA, //Irish
    GD, //Scots Gaelic
    GL, //Galician
    GN, //Guarani
    GU, //Gujarati
    HA, //Hausa
    HI, //Hindi
    HE, //Hebrew
    HR, //Croatian
    HU, //Hungarian
    HY, //Armenian
    IA, //Interlingua
    ID, //Indonesian
    IE, //Interlingue
    IK, //Inupiak
    IN, //former Indonesian
    IS, //Icelandic
    IT, //Italian
    IU, //Inuktitut (Eskimo)
    IW, //former Hebrew
    JA, //Japanese
    JI, //former Yiddish
    JW, //Javanese
    KA, //Georgian
    KK, //Kazakh
    KL, //Greenlandic
    KM, //Cambodian
    KN, //Kannada
    KO, //Korean
    KS, //Kashmiri
    KU, //Kurdish
    KY, //Kirghiz
    LA, //Latin
    LN, //Lingala
    LO, //Laothian
    LT, //Lithuanian
    LV, //Latvian, Lettish
    MG, //Malagasy
    MI, //Maori
    MK, //Macedonian
    ML, //Malayalam
    MN, //Mongolian
    MO, //Moldavian
    MR, //Marathi
    MS, //Malay
    MT, //Maltese
    MY, //Burmese
    NA, //Nauru
    NE, //Nepali
    NL, //Dutch
    NO, //Norwegian
    OC, //Occitan
    OM, //(Afan) Oromo
    OR, //Oriya
    PA, //Punjabi
    PL, //Polish
    PS, //Pashto, Pushto
    PT, //Portuguese
    QU, //Quechua
    RM, //Rhaeto-Romance
    RN, //Kirundi
    RO, //Romanian
    RU, //Russian
    RW, //Kinyarwanda
    SA, //Sanskrit
    SD, //Sindhi
    SG, //Sangro
    SH, //Serbo-Croatian
    SI, //Singhalese
    SK, //Slovak
    SL, //Slovenian
    SM, //Samoan
    SN, //Shona
    SO, //Somali
    SQ, //Albanian
    SR, //Serbian
    SS, //Siswati
    ST, //Sesotho
    SU, //Sudanese
    SV, //Swedish
    SW, //Swahili
    TA, //Tamil
    TE, //Tegulu
    TG, //Tajik
    TH, //Thai
    TI, //Tigrinya
    TK, //Turkmen
    TL, //Tagalog
    TN, //Setswana
    TO, //Tonga
    TR, //Turkish
    TS, //Tsonga
    TT, //Tatar
    TW, //Twi
    UG, //Uigur
    UK, //Ukrainian
    UR, //Urdu
    UZ, //Uzbek
    VI, //Vietnamese
    VO, //Volapuk
    WO, //Wolof
    XH, //Xhosa
    YI, //Yiddish
    YO, //Yoruba
    ZA, //Zhuang
    ZH, //Chinese
    ZU  //Zulu

}


public static class StringExtensions
{


    public static string UnescapeXML(this string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        string returnString = s;
        returnString = returnString.Replace("&apos;", "'");
        returnString = returnString.Replace("&quot;", "\"");
        returnString = returnString.Replace("&gt;", ">");
        returnString = returnString.Replace("&lt;", "<");
        returnString = returnString.Replace("&amp;", "&");

        return returnString;
    }
}
#endregion