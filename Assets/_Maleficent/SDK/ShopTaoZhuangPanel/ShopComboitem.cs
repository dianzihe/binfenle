using UnityEngine;
using System.Collections;

public class ShopComboitem : MonoBehaviour 
{
	public int m_nIndex=-1;
	public UILabel title=null;
	public UILabel price=null;

	public void SetIndex(int nIndex)
	{
		m_nIndex = nIndex;
	}
	public int GetIndex()
	{
		return m_nIndex;
	}
	public UIEventListener m_listener ;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
