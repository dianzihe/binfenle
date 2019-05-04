using UnityEngine;
using System.Collections;
 
public class SDKMonbehavior : MonoBehaviour 
{

	string m_strTalkingAppID="D35A5C7225AB314DB1C70A40A7511123";
	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}
	// Use this for initialization
	void Start () 
	{

		int nMusicToggle = PlayerPrefs.GetInt ("Music", -1);
		if(nMusicToggle!=-1)
		{
			nMusicToggle=0;
		}
		PlayerPrefs.SetInt ("Music", 0);


		FlurrtSDKUtil.GetInstance ().StartSDKSession ("","");
		//FlurrtSDKUtil.GetInstance ().SetAccount ();

		 
		TalkingGameSDKUtil.GetInstance ().StartSDKSession(m_strTalkingAppID,"100006");
		TalkingGameSDKUtil.GetInstance ().SetAccount ();
 
		////
		/// 

	}

 
	// Update is called once per frame
	void Update ()
	{
	
	}
}
