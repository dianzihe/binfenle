using UnityEngine;
using System.Collections;

public class ColliderManager : MonoBehaviour {

	static string m_strTAG="MAPBOOK_COLL_TAG";
	// Use this for initialization
	void Start () 
	{
	
	}

	public static void ToggleCollider(bool bTog)
	{
		GameObject [] colArray = GameObject.FindGameObjectsWithTag (m_strTAG);
		if(colArray!=null)
		{
			for(int i=0;i<colArray.Length;i++)
			{
				BoxCollider col=colArray[i].GetComponent<BoxCollider>();
				if(col!=null)
				{
					col.enabled=bTog;
				}
			}
		}
		GameObject mapCam = GameObject.Find ("Map Camera");
		if(mapCam!=null)
		{
			BravoInputManager man=mapCam.GetComponent<BravoInputManager>();
			if(man)
			{

					man.enabled=bTog;
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
	
	}
}
