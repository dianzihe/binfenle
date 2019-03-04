using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class LoadPlayerPrefs : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		if (UserManagerCloud.Instance != null) {
			UserManagerCloud.Instance.LoadUserFromCloud();
		}
		
		LocalizationServerManager.Instance.DownloadLanguages();
		TweaksSystemManager.Instance.SynchTweaks();
		
//		PlayerPrefs.DeleteAll();
		
		//BEGIN PRICES REGEX TEST
//		string[] prices = {"USD2", "99$", ".99$", "EUR200.999", "2,000$", "USD1,999.99", "100.000.000 EUR", "EUR 100,000,000.99"};
//		string priceValue;
//		
//		Regex regex = new Regex("(?<price>([0-9]*[.,]?[0-9]+)+)");
//		
//		foreach (string price in prices) {
//	        Match match = regex.Match(price);
//	
//	        if (match.Success)
//	        {
//	            priceValue = match.Groups["price"].Value;
//				Debug.LogWarning(priceValue + "     " + price.Replace(priceValue ,""));
//	        }
//		}
//		Debug.LogError("Finished test");
		//END PRICES REGEX TEST
	}
}

