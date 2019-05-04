using UnityEngine;
using System.Collections;

public class SinaSharePanel : MonoBehaviour 
{
	public PlayMakerFSM m_fsm=null;
	public UIEventListener m_close=null,m_send=null;
	public UITexture m_tex=null;
	public UIInput m_text=null;
	static private  SinaSharePanel m_ins=null;
	static public SinaSharePanel getInstance()
	{
		return m_ins;
	}

	void Awake()
	{
		m_ins = this;
		m_ins.gameObject.SetActive (false);
	}

	public void OpenPanel()
	{
		gameObject.SetActive (true);
		StartCoroutine (LoadPic ());
	}
	// Use this for initialization
	void Start () 
	{
		if(m_close!=null)
		{
			m_close.onClick=ClosePanel;
		}

		if(m_send!=null)
		{
			m_send.onClick=SendInfor;
		}
	
	}

	IEnumerator  LoadPic()
	{
		WWW www = new WWW ("file://"+Application.persistentDataPath+ "/sharepic.png");
		yield return www.texture;
		m_tex.mainTexture= www.texture;
	}

	void ClosePanel(GameObject go)
	{
		gameObject.SetActive (false);
	}
	void SendInfor(GameObject go)
	{
		#if UNITY_ANDROID

		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			Debug.Log("SinaActivity");
			using( AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				Debug.Log("SinaShare");
				jo.Call("Unity_SinaShare",m_text.text,Application.persistentDataPath+"/sharepic.png");
			}
		}
		
		#endif
	}
	// Update is called once per frame
	void Update () {
	
	}
}
