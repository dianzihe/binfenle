using UnityEngine;
using System.Collections;

public class ShopPropertyItem : MonoBehaviour 
{
	public UIEventListener m_btnListener = null;
	public  UILabel m_lab=null;
	public UISprite m_spr;
	public int m_nIndex;
	public UILabel m_labName=null;
	public UILabel m_costLab=null;
	
	public void LoadSprite(string strPicName)
	{

		m_spr.spriteName = strPicName;
	}
	public void LoadLabel(UILabel lab,string strLabel)
	{
		lab.text = strLabel;
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
