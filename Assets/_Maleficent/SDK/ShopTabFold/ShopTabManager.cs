using UnityEngine;
using System.Collections;

public class ShopTabManager : MonoBehaviour 
{

	// Use this for initialization
	public UIEventListener m_combo,m_mana,m_property;
	public UISprite m_Combo_clk_spr,m_Mana_clk_spr,m_Property_clk_spr;
	void Start () 
	{
		if(m_combo!=null)
			m_combo.onClick = OnCombo;
		if(m_mana!=null)
			m_mana.onClick = OnMana;
		if(m_property!=null)
			m_property.onClick = OnProperty;
	}
	void OnCombo(GameObject go)
	{
		ManagerBtnState (0);
		NotifyUI (0);
	}
	public void ManagerBtnState(int nbtnIndex)
	{
		switch(nbtnIndex)
		{
		case 0:
			m_Combo_clk_spr.alpha=255;
			m_Mana_clk_spr.alpha=0;
			m_Property_clk_spr.alpha=0;
			break;
		case 1:
			m_Mana_clk_spr.alpha=255;
			m_Combo_clk_spr.alpha=0;
			m_Property_clk_spr.alpha=0;
			break;
		case 2:
			m_Property_clk_spr.alpha=255;
			m_Combo_clk_spr.alpha=0;
			m_Mana_clk_spr.alpha=0;
			break;
		}
	}
	void OnMana(GameObject go)
	{
		ManagerBtnState (1);
		NotifyUI (1);
	}
	void OnProperty(GameObject go)
	{
		ManagerBtnState (2);
		NotifyUI (2);
	}
	void NotifyUI(int nindex)
	{
		if(ShopUINotifyer.getInstance()!=null)
		{
			ShopUINotifyer.getInstance().IndexUI=nindex;
			ShopUINotifyer.getInstance().Notify();
		}
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
