using UnityEngine;
using System.Collections;

public class InfoSaveFode : MonoBehaviour {
	public bool issigntoday;
	public static InfoSaveFode instanse=null;
	public int getcount;
	// Use this for initialization
	void Start () {
		instanse = this;
//		issigntoday = SigninSystem.getinstanse ().issign;
//		getcount = SigninSystem.getinstanse ().count;
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
