using UnityEngine;
using System.Collections;

public class ManaLabel : MonoBehaviour
{
	UILabel label;
	
	void Start () 
	{
		GetComponent<UILabel>().text = Language.Get("MANA_WON");
	}
}

