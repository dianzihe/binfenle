using UnityEngine;
using System.Collections;

public class Giftnotshow : MonoBehaviour {

	//public int notshowgift=1;
	public static Giftnotshow instanse=null;

	//public bool 
	public static Giftnotshow getinstanse()
	{
		return instanse;
	}
	void Start () {
		GiftIsShow.getinstanse ().showgift = 1;
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {

		//	SignServerReadText.GetInstanse ().issigned = true;
//		if (InfoSaveFode.instanse.issigntoday) {
//			GiftIsShow.getinstanse ().issign = true;
//			print("change");	
//		}

	}
}
