using UnityEngine;
using System.Collections;

public class GiftIsShow : MonoBehaviour {
//	public UIEventListener playclick;
	// Use this for initialization
	int m_showgift=2;
	public bool issign = true;
	public int showgift
	{
		get {return m_showgift;}
		set 
		{
			m_showgift = value;
		}
	}
	public static GiftIsShow instanse=null;
	public static GiftIsShow getinstanse()
	{
		return instanse;
	}
	void Awake () {
		instanse = this;
		DontDestroyOnLoad (gameObject);
	}
	
	// Update is called once per frame
	void Update () {

	}

}
