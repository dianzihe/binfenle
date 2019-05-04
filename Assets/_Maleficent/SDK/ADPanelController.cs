using UnityEngine;
using System.Collections;

public class ADPanelController : MonoBehaviour
{
	const string m_strURL = "http://i.disney.cn/pms/IOSBXQY.p?p=M";
	public GameObject m_ADTexture=null;

	// Use this for initialization
	void Start () 
	{
		m_ADTexture.GetComponent<UIEventListener> ().onClick = OnADTextureClkEvent;
	
	}
	void OnADTextureClkEvent(GameObject go)
	{
		Application.OpenURL (m_strURL);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
