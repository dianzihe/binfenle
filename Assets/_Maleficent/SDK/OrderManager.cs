using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour 
{
	public static string m_CurrentOrderID="";
	static HashSet<string> m_strHash=new HashSet<string>();
	static OrderManager m_Instance=null;
	public static OrderManager getInstance()
	{
		if(m_Instance==null)
		{
			m_Instance= new OrderManager();
			return m_Instance;
		}
		return m_Instance;
	}
	private OrderManager()
	{

	}
	public bool AddOrderLst(string OrderID)
	{
		if(m_strHash.Contains (OrderID)==true)
		{
			return false;
		}
		else
		{
			m_strHash.Add(OrderID);
			m_CurrentOrderID=OrderID;
			return true;
		}
	}
	public void RemoveOrderLst(string strOrderID)
	{
		m_strHash.Remove (strOrderID);
	}

	// Use this for initialization
	void Start () 
	{
	
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
