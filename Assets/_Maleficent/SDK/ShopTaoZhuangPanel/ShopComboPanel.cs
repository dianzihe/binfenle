using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ShopComboPanel : MonoBehaviour
{

	private static ShopComboPanel m_instance=null;
	private static  Dictionary<int,ShopComboitem> m_dict=new Dictionary<int, ShopComboitem>();
	ShopComboitem [] Itemarray;
	public Transform m_parent=null;
	// Use this for initialization
	void Start () 
	{
	
	}
	public  static ShopComboPanel getInstance()
	{
		return m_instance;
	}
	void Awake()
	{
		m_instance = this;
	}
	public void HideUI()
	{
		gameObject.SetActive (false);
	}
	public void ShowUI()
	{
		gameObject.SetActive (true);
		GetBtnDelegate ();
	}
	public void GetBtnDelegate()
	{
		Itemarray = m_parent.gameObject.GetComponentsInChildren<ShopComboitem> ();
		if(Itemarray!=null)
		{
			for(int i=0;i<Itemarray.Length;i++)
			{
				Itemarray[i].m_listener.onClick=OnComboClick;
			}
		}
	}
	void OnComboClick(GameObject go)
	{
		GameObject go_par = go.transform.parent.gameObject;
		GameObject go_par_par = go_par.transform.parent.gameObject;
		ShopComboitem item = go_par_par.GetComponent<ShopComboitem> ();
		if(item!=null)
		{
			ShopBuyBoxPanel.getInstance().ShowUI(item.title.text,item.price.text,LoadStr(item),item.m_nIndex);
		}

	}
	string LoadStr(ShopComboitem item)
	{
		string str = "";
		switch(item.m_nIndex)
		{
		case 1:
			str="low_gift";
			break;
		case 2:
			str="medium_gift";
			break;
		case 3:
			str="hign_gift";
			break;
		}
		return str;
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
}
