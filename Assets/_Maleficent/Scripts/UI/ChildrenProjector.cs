using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ChildrenProjector : MonoBehaviour
{
	// Use this for initialization
	public void ProjectChildren ()
	{
		ChaptersManager.Instance.GetPagesCollider().enabled = true;
		foreach(Transform child in transform)
		{
			RaycastHit hitInfo;
			if(Physics.Raycast(child.transform.position,Vector3.down,out hitInfo,50.0f))
				child.transform.position += Vector3.down*(hitInfo.distance-0.05f);
		}
		ChaptersManager.Instance.GetPagesCollider().enabled = false;
	}
}


