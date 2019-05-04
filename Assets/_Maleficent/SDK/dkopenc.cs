using UnityEngine;
using System.Collections;

public class dkopenc : MonoBehaviour {
	public bool issupport=false;
	string platform;
	changeres responseOBj;
	public string[] PlatformArray;
	public bool isrightPlat;
	string platresult;
	string URLpath="http://maleficent.zooking.cn:8080/SleepCurse/exchange";
	//string URLpath="http://192.168.50.36:8080/SleepCurse/isOpenExchange";

	void Awake(){
		responseOBj = GameObject.Find ("Code").GetComponent<changeres> ();
	}
	// Use this for initialization
	void Start () {

		#if UNITY_IPHONE
		platform="IPHONE";
		StartCoroutine(CheckPlatform(platform));
		#endif

		#if UNITY_ANDROID

		#endif
	}
	
	// Update is called once per frame
	void Update () {

	}
	void OnClick(){



	}
	IEnumerator CheckPlatform(string resultPlat){
//		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
//		{
//			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("CChttppost"))
//			{
//				string result;
//				if(jo!=null)
//				{
//					jo.Call("issupport",URLpath,resultPlat); 
//					Debug.Log("resultPlat="+resultPlat);
//				}
//				else
//					Debug.Log("Init is NULL!");
//			}
//		}
		WWWForm wform = new WWWForm ();
		wform.AddField ("platform", resultPlat);
		WWW www = new WWW (URLpath,wform);
		yield return www;
		if (www.error != null) {
			platresult = "error:" + www.error;	
		} else {
			platresult=www.text;	
		}
		if(platresult=="false"){
			//Debug.Log("IPHONEresult="+pfresult);
			//transform.GetComponent<NGuiEventsToPlaymakerFsmEvents>().enabled=false;
			//GameObject.Find("UI Root Landscape/Change Camera").SetActive(false);
			GameObject.Find("UI Root Portrait/Change Camera").SetActive(false);
			//Debug.Log("Don't Show Iphone");
		}
		else{
			//Debug.Log("Support Iphone");
			//GameObject.Find("UI Root Landscape/Change Camera").SetActive(true);
			GameObject.Find("UI Root Portrait/Change Camera").SetActive(true);
			//Debug.Log("Iphone Platform");
		}
	}
	void Hand_ChangeCode_OK_CallBack(){

	}
	void Hand_ChangeCode_Cancel_CallBack(){

	}
}
