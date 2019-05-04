using UnityEngine;
using System.Collections;

public class ClickerListener : MonoBehaviour 
{

	public Camera m_backcam=null;
	public Camera m_manacam=null;
	public PlayMakerFSM m_fsm=null;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void OnMouseDown()
	{
		if (Input.GetMouseButton (0))     
		{    
			Ray ray = m_manacam.ScreenPointToRay (Input.mousePosition);    
			RaycastHit  hit;     
			if (Physics.Raycast (ray,out  hit))     
			{    
				if("Image Button"==hit.collider.gameObject.name)
				{
					return;
				}

			}  
			ray = m_backcam.ScreenPointToRay (Input.mousePosition);    
			if (Physics.Raycast (ray,out  hit))     
			{    
				if("Background"==hit.collider.gameObject.name)
				{
					m_fsm.SendEvent("BackGame");
				}
			}    
		}    
	}


}
