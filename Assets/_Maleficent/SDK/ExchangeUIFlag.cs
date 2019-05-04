using UnityEngine;
using System.Collections;

public class ExchangeUIFlag : MonoBehaviour 
{
	public PlayMakerFSM m_shop;
	public bool m_bShow=true;
	public GameObject showObj;
	public void setShow()
	{
		m_bShow = false;
		ColliderManager.ToggleCollider (false);
	}
	public void reflectShow()
	{
		m_bShow = true;
		ColliderManager.ToggleCollider (true);
	}
	static public ExchangeUIFlag getInstance()
	{
		return m_instance;
	}
	static private ExchangeUIFlag m_instance=null;
	void Awake()
	{
		m_instance=this;
	}
	// Use this for initialization
	void Start () {
	
	}
	public void setSelfVisiable()
	{
		if(GiftBagController.getInstance()!=null)
		{
			GiftBagController.getInstance().gameObject.SetActive(true);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
	void CheckShow()
	{
			//showbuy();
			//GameObject.Find("ShopUI").gameObject.SetActive(true);
			//showObj.gameObject.SetActive(true);
			ShopUINotifyer.getInstance ().IndexUI = 2;
			ShopUINotifyer.getInstance ().Notify ();
			m_shop.SendEvent("GameShopBegin");
			print("buy live");


	}

}
