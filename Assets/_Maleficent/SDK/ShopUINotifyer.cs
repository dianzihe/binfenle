using UnityEngine;
using System.Collections;

public class ShopUINotifyer : MonoBehaviour 
{
	public static ShopUINotifyer getInstance()
	{
		return m_instance;
	}
	private static ShopUINotifyer m_instance=null;
	void Awake()
	{
		m_instance = this;
	}
	protected static int m_nUIIndex;
	public int IndexUI
	{
		set
		{
			if(value<3)
			{
				m_nUIIndex=value;
			}
		}
		get
		{
			return m_nUIIndex;
		}
	}
	public void Notify()
	{
		if(ShopPanel.getInstance().gameObject.activeSelf!=false)
		{
			ShopPanel.getInstance().getNotifyToShow(m_nUIIndex);
		}
	}
	public void PropertyUI()
	{
		m_nUIIndex = 2;
		Notify ();
	}
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
