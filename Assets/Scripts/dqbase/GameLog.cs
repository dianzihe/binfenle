using UnityEngine;
using System;
using System.Collections;
using System.Text;
using System.IO;
using Logic;

public class CLog : tLog
{
	//static UITextList debugTextUi;
	static public GUIText mLogObject;
	
	public CLog()
	{
		#if RELEASE_WEB
		
		#else
		if (File.Exists(mLogFileName))
			File.Delete(mLogFileName);
		
		Log(mLogFileName);
		
		#endif
	}
	
	//static public void Init()
	//{
	//    //UiControl.self.ShowDebugInfoPanel(true);
	//    //UiControl.self.ShowFramePanel(true);
	//    GameObject obj = GameObject.Find("InfoTextList") as GameObject;
	//    if (obj!=null)
	//    {
	//        obj.SetActiveRecursively(true);
	//        debugTextUi = obj.GetComponentInChildren<UITextList>();
	//    }
	
	//}
	
	#if UNITY_IPHONE || UNITY_ANDROID || DEBUG_IO
	public static string mLogFileName = GameCommon.MakeGamePathFileName("GameRun.txt");
	#else
	public static string mLogFileName = GameCommon.MakeGamePathFileName("../GameRun.txt");
	//public static string mLogFileName = "e:/GameRun.txt";
	#endif
	public override void Log(string info)
	{
        if (!CommonParam.NeedLog)
            return;

		//GameObject g = GameObject.Find("DebugInfo2");
		//if (null != mLogObject)
		//{
		//    GUIText gt = mLogObject.GetComponent<GUIText>();
		//    if (gt!=null)
		//        gt.text += info + "\n\t";
		//}
		
		if (mLogObject != null)
			mLogObject.text += info + "\n\t";
		
		
		//Debug.Log( info );
		#if RELEASE_WEB
		//if (debugTextUi == null)
		//    Init();
		//if (debugTextUi != null)
		//    debugTextUi.Add(info);
		
		#else
		bool b = File.Exists(mLogFileName);
		StreamWriter writer = new StreamWriter(mLogFileName, b, Encoding.Unicode);
		writer.WriteLine(DateTime.Now.ToString("T") + ">" + info);        
		//writer.Write("\r\n");
		writer.Close();
		#endif
	}
	public GameObject log;
	
	
}