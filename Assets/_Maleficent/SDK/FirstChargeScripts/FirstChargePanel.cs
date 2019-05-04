using UnityEngine;
using System.Collections;

public class FirstChargePanel : MonoBehaviour
{
	public PlayMakerFSM m_fsmFirstCharge=null;
	public FirstChargeContentPanel m_contentPanel=null;
	public UIEventListener m_FirstBtn=null;
	static public FirstChargePanel getInstance()
	{
		return m_ins;
	}
	static private FirstChargePanel m_ins=null;
	void Awake()
	{
		//TestCharge();
		m_ins=this;
		IsFirstChargeBtnVisiable();
	}
	// Use this for initialization
	void TestCharge()
	{
		PlayerPrefs.DeleteKey("FirstCharge_Already_Ready");
		PlayerPrefs.Save();
	}

	public bool IsFirstChargeBtnVisiable()
	{
		int nFirstChargeFlag=0;
		nFirstChargeFlag=PlayerPrefs.GetInt("FirstCharge_Already_Ready",-1);
		if(nFirstChargeFlag==-1)
		{
			m_FirstBtn.gameObject.SetActive(true);
			//PlayerPrefs.SetInt("FirstCharge_Already_Ready",1);
		}
		else if(nFirstChargeFlag==1)
		{
			m_FirstBtn.gameObject.SetActive(false);
		}
		return true;
	}
	void Start () 
	{
		if(m_FirstBtn!=null)
			m_FirstBtn.onClick=OnFirstChargeBtnClick;
	}
	public void ColSetter(bool bTog)
	{
		m_FirstBtn.GetComponent<Collider>().enabled=bTog;
	}
	void OnFirstChargeBtnClick(GameObject go)
	{
		if(m_contentPanel!=null)
		{
			m_contentPanel.ShowWindow(true);
			if(m_fsmFirstCharge!=null)
			{
				m_fsmFirstCharge.SendEvent("FirstChargeBegin");
			}
		}
		//m_contentPanel.
		//PlayerPrefs.SetInt("FirstCharge_Already_Ready",1);
	}
	public void ClsFsm()
	{
		if(m_fsmFirstCharge!=null)
		{
			m_fsmFirstCharge.SendEvent("CloseUI");
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
