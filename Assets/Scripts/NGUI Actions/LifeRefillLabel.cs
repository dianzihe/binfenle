using UnityEngine;
using System.Collections;

public class LifeRefillLabel : MonoBehaviour 
{
	UILabel label;
	
	void Start () 
	{
		GetComponent<UILabel>().text = Language.Get("NEXT_LIFE");
	}
}
