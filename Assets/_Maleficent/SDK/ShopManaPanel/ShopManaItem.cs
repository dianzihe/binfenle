using UnityEngine;
using System.Collections;

public class ShopManaItem : MonoBehaviour 
{
	public UIEventListener m_btnListener = null;
	public UILabel m_labName;
	public  UILabel m_lab=null,m_priceLab=null;
	private int m_addMana;
	public UISprite m_spr;
	public int m_nIndex;
	public int ManaPoint
	{
		get
		{
			return m_addMana;
		}
		set
		{
			m_addMana=value;
		}
	}

	public void LoadSprite(string strPicName)
	{
		m_spr.spriteName = strPicName;
	}
	public void LoadLabel(string strLabel)
	{
		m_lab.text = strLabel;
	}
	public void LoadPriceLabel(string strLabel)
	{
		m_priceLab.text = strLabel;
	}
	public void LoadNameLabel(string strLabel)
	{
		m_labName.text = strLabel;
	}
	public void SetIndex(int nIndex)
	{
		m_nIndex = nIndex;
	}
	// Use this for initialization
	void Start () 
	{
	
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
