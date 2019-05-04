using UnityEngine;
using System.Collections;
 
public class ShareToolManager : MonoBehaviour 
{
	private static ShareToolManager m_instance=null;
	public static ShareToolManager getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
	}
	public UIEventListener m_wechat,m_sinablog;
	void OnShareWeChat(GameObject sender)
	{
		WechatShare();
		//PicGenerator ();
		//ShowMenu (PlatformType.WeChatTimeline);
	}

	void OnShareSinaBlog(GameObject sender)
	{
		//PicGenerator ();
		//ShowMenu (PlatformType.SinaWeibo);
		SinaSharePanel.getInstance ().OpenPanel ();
	}

	// Use this for initialization
	void Start () 
	{
		if(m_wechat!=null)
		{
			m_wechat.onClick=OnShareWeChat;
		}
		if(m_sinablog!=null)
		{
			m_sinablog.onClick=OnShareSinaBlog;
		}


	}
 

	public static void PicGenerator()
	{
		//Application.CaptureScreenshot ("sharepic.png");
	}

	void WechatShare()
	{
		#if UNITY_ANDROID
		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			Debug.Log("Init:CustomUnityPlayerActivity");
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperWeChat"))
			{
				Debug.Log("Init:payWrapperWeChat");
				jo.Call("Unity_Init");
			}
		}

		using (AndroidJavaClass jc = new AndroidJavaClass("com.disney.maleficentchina.CustomUnityPlayerActivity"))
		{
			Debug.Log("CustomUnityPlayerActivity");
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("payWrapperWeChat"))
			{
				Debug.Log("payWrapperWeChat");
				
				jo.Call("Unity_SharePic",Application.persistentDataPath+"/sharepic.png"); 
				
			}
		}

		#endif

	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
