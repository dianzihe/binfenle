using UnityEngine;
using System.Collections;

public class GiftBagController : MonoBehaviour 
{
	public GiftBagPanel m_paenl;
	public BoxCollider []m_colWidget;
	public GameObject life_go, store_go, shop_go,exchange_go,info_go;
	private int m_nShowWhichItem = -1;
	public UIEventListener m_lis=null;
	private static GiftBagController m_instance=null;
	public static GiftBagController getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		//just for test
		//TestGift();
		m_instance = this;
 		CheckGift ();
	}
	public void ToggleWidgetCol(bool bTog)
	{
//		for(int i=0;i<m_colWidget.Length;i++)
//		{
//			m_colWidget[i].enabled=bTog;
//		}
		ColliderManager.ToggleCollider (bTog);
	}

	void TestGift()
	{
		PlayerPrefs.DeleteKey("Gift_5");
		PlayerPrefs.DeleteKey("Gift_6");
		PlayerPrefs.Save();
	}
	void CheckGift()
	{

		string strGift_1_Value=PlayerPrefs.GetString ("Gift_5","");
		string strGift_2_Value=PlayerPrefs.GetString ("Gift_6","");
		if(strGift_1_Value=="")
		{
			PlayerPrefs.SetString("Gift_5","0");
			PlayerPrefs.SetString("Gift_6","0");
			m_nShowWhichItem=1;
		}
		else if(strGift_1_Value=="0")
		{
			m_nShowWhichItem=1;
		}
		else if(strGift_1_Value=="1"&&strGift_2_Value=="0")
		{
			m_nShowWhichItem=2;
		}
		else if(strGift_1_Value=="1"&&strGift_2_Value=="1")
		{
			m_nShowWhichItem=0;
		}



	}

	public void OtherUI(bool bshow)
	{
		/*
			life_go.SetActive(bshow);
			store_go.SetActive(bshow);
			shop_go.SetActive(bshow);
			exchange_go.SetActive (bshow);
			info_go.SetActive (bshow);
		*/	
			ToggleWidgetCol(true);
			//FirstChargePanel.getInstance().ColSetter(true);
	}
	void Show()
	{
		m_instance.gameObject.SetActive (true);
		CheckGift ();

		if(m_nShowWhichItem!=0)
			m_paenl.ShowUI (m_nShowWhichItem);
		else
			Hide();

	//	OtherUI (false);
	}
	public	void Hide()
	{
		m_instance.gameObject.SetActive (false);
		ToggleWidgetCol(true);
	//	FirstChargePanel.getInstance().ColSetter(true);
	//	OtherUI (true);
	}

	public void ShowOtherUI()
	{
		OtherUI (true);
	}
	// Use this for initialization
	void Start () 
	{
		if(m_lis!=null)
		{
			m_lis.onClick=OnShowGift;
		}
	}
	void OnShowGift(GameObject go)
	{
		Show ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_nShowWhichItem==0)
		{
			Hide();
		}
	}
}
