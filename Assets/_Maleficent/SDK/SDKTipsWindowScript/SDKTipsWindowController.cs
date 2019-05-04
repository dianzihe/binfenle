using UnityEngine;
using System.Collections;

public class SDKTipsWindowController : MonoBehaviour 
{
	public GameObject m_tabGO=null,m_closeGO;
	static SDKTipsWindowController mInstance=null;
	public static SDKTipsWindowController getInstance()
	{
		return mInstance;
	}

	void Awake()
	{

		mInstance = gameObject.GetComponent<SDKTipsWindowController>();
		DontDestroyOnLoad (gameObject);
	}
	public UILabel m_labDesc=null;
	public delegate void OkCallback();
	public OkCallback m_okNotify;
	public delegate void CancelCallback();
	public CancelCallback m_cancelNotify;
	public GameObject m_root=null;
	public UIEventListener m_okBtn=null,m_cancelBtn=null;
	public UILabel m_okBtnLab,m_cancelBtnLab;
	public GameObject m_background=null;
	// Use this for initialization
	void Start () 
	{

		if (m_okBtn != null) 
		{
			m_okBtn.onClick=HandleOKClick;
			if(m_okBtnLab!=null)
			{
				m_okBtnLab.text=Language.Get("YES");
			}
		}
		if (m_cancelBtn != null) 
		{
			m_cancelBtn.onClick=HandleCancelClick;
			if(m_cancelBtnLab!=null)
			{
				m_cancelBtnLab.text=Language.Get("PRICE_CANCEL");
			}
		}
		m_root.transform.localScale=new Vector3(0.75f,0.75f,0.75f);

		OrientationListener.Instance.OnOrientationChanged += HandleOrientChanged;
		
	}
	void EnableInput(bool bInput)
	{
		BoxCollider [] col = m_tabGO.GetComponentsInChildren<BoxCollider> ();
		if(col!=null)
		{
			for(int i=0;i<col.Length;i++)
			{
				col[i].enabled=bInput;
			}
		}
		if(m_closeGO!=null)
		{
			m_closeGO.GetComponent<BoxCollider>().enabled=bInput;
		}
	}
	void OnDestroy()
	{
		OrientationListener.Instance.OnOrientationChanged -= HandleOrientChanged;
	}
	void HandleOrientChanged(ScreenOrientation newOrientation)
	{

		if (newOrientation == ScreenOrientation.Landscape) 
		{
			m_root.transform.localScale=new Vector3(1f,1f,1f);
		}
		else if(newOrientation == ScreenOrientation.Portrait)
		{
			m_root.transform.localScale=new Vector3(0.75f,0.75f,0.75f);
		}
	}
	void InitDesc(string strLabel)
	{
		if(m_labDesc!=null)
		{
			m_labDesc.text=strLabel;
		}
	}

	public void PopWindow(string strLabel,OkCallback okcallback,CancelCallback cancelcallback)
	{
		InitDesc (strLabel);
		m_okNotify = okcallback;
		m_cancelNotify = cancelcallback;
		if(m_okNotify==null)
		{
			m_okBtn.gameObject.SetActive(false);
		}
		else
		{
			m_okBtn.gameObject.SetActive(true);
		}
		if(m_cancelNotify==null)
		{

			m_cancelBtn.gameObject.SetActive(false);
			if(m_okNotify!=null)
			{
				m_okBtn.gameObject.transform.localPosition=new Vector3(187,0,0);
			}
		}
		else
		{
			m_cancelBtn.gameObject.SetActive(true);
			m_okBtn.gameObject.transform.localPosition=new Vector3(319,0,0);
		}
		GameObject lis = GameObject.Find ("BWCamera");
		if(lis!=null)
		{
			ClickerListener comClick=lis.GetComponentInChildren<ClickerListener>();
			if(comClick!=null)
			{
				comClick.enabled=false;
			}
			BoxCollider colbox=lis.GetComponentInChildren<BoxCollider>();
			if(colbox!=null)
			{
				colbox.enabled=false;
			}
		}

		EnableInput (false);
		ActiveUI ();
	}

	void ActiveUI()
	{
		m_root.SetActive (true);
	}

	void DestroyUI()
	{
		m_root.SetActive (false);

		GameObject lis = GameObject.Find ("BWCamera");
		if(lis!=null)
		{
			ClickerListener comClick=lis.GetComponentInChildren<ClickerListener>();
			if(comClick!=null)
			{
				comClick.enabled=true;
			}
			BoxCollider colbox=lis.GetComponentInChildren<BoxCollider>();
			if(colbox!=null)
			{
				colbox.enabled=true;
			}
		}
		EnableInput (true);
	}

	void HandleOKClick(GameObject go)
	{
		m_okNotify ();
		DestroyUI ();
	}

	void HandleCancelClick(GameObject go)
	{
		m_cancelNotify ();
		DestroyUI ();
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
