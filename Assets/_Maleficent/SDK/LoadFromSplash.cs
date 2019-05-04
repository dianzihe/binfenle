using UnityEngine;
using System.Collections;

public class LoadFromSplash : MonoBehaviour {
	
	
	public int m_nSec=1 ;
	public string m_strSceneName="";
	// Use this for initialization
	void Start () 
	{
		Invoke("Load",m_nSec);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Load()
	{
		Application.LoadLevel(m_strSceneName);
	}
}
