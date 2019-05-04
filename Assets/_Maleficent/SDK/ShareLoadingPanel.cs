using UnityEngine;
using System.Collections;

public class ShareLoadingPanel : MonoBehaviour 
{
	private static ShareLoadingPanel m_ins=null;
	public static ShareLoadingPanel getInstance()
	{
		return m_ins;
	}
	void Awake()
	{
		m_ins = this;
		gameObject.SetActive (false);
	}
	public void ShowLoadingPanel(bool bShow)
	{
		gameObject.SetActive (bShow);
	}


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	
	}
}
