using UnityEngine;
using System.Collections;

public class ShopPanel : MonoBehaviour 
{
	public BoxCollider m_infoToggle=null;
	static private ShopPanel m_instance = null;
	public PlayMakerFSM m_fsm;
	public string m_strEventClose;
	public UIEventListener m_closeBtnDelegate = null;
	public UILabel m_labManaValue=null;
	private int m_nManaValue;
	public void InitValueMana()
	{
		m_nManaValue = TokensSystem.Instance.ManaPoints;
		m_labManaValue.text = m_nManaValue.ToString ();
		//m_nManaValue
	}
	static public ShopPanel getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
		m_instance.gameObject.SetActive (false);
		InitValueMana ();
	}

	public ShopManaPanel m_manaPanel=null;
	public ShopComboPanel m_comboPanel=null;
	public ShopPropertyPanel m_PropertyPanel=null;

	// Use this for initialization
	void Start () 
	{
		if(m_closeBtnDelegate!=null)
		{
			m_closeBtnDelegate.onClick=OnCloseClick;
		}
	}
	public void OnCloseClick(GameObject go)
	{
		m_fsm.SendEvent (m_strEventClose);
		if(GiftBagController.getInstance()!=null)
		{
			GiftBagController.getInstance().gameObject.SetActive(true);
		}
		if(m_infoToggle!=null)
		{
			m_infoToggle.enabled=true;
		}

		if(FirstChargeContentPanel.getInstance()!=null)
		{
			if(FirstChargeContentPanel.getInstance().gameObject.activeSelf==false)
				ColliderManager.ToggleCollider (true);
			else
			{
				GameObject mapCam = GameObject.Find ("Map Camera");
				if(mapCam!=null)
				{
					BravoInputManager man=mapCam.GetComponent<BravoInputManager>();
					if(man)
					{
						
						man.enabled=false;
					}
				}
			}
		}
		/* new add */
			if(Match3BoardGameLogic.Instance!=null)
			{
				if(Match3BoardGameLogic.Instance.m_bShowLosePanel==true)
				{
					Match3BoardGameLogic.Instance.ShowLoseEvent();
			    }
			}



	}
	public void getNotifyToShow(int i)
	{
		ShopTabManager manager=gameObject.GetComponentInChildren<ShopTabManager> ();
		if(manager!=null)
		{
			manager.ManagerBtnState(i);
		}
		switch(i)
		{
		case 0:

			m_PropertyPanel.HideUI();
			m_manaPanel.HideUI();
			m_comboPanel.ShowUI();
			break;
		case 1:
			m_PropertyPanel.HideUI();
			m_comboPanel.HideUI();
			m_manaPanel.ShowUI();
			break;
		case 2:
			m_comboPanel.HideUI();
			m_manaPanel.HideUI();
			m_PropertyPanel.ShowUI();
			break;
		}
		if(GiftBagController.getInstance()!=null)
		{
			GiftBagController.getInstance().gameObject.SetActive(false);
		}
		if(m_infoToggle!=null)
		{
			m_infoToggle.enabled=false;
		}

		ColliderManager.ToggleCollider (false);

	}
	
	// Update is called once per frame
	void Update () 
	{
		InitValueMana ();
	}
}
