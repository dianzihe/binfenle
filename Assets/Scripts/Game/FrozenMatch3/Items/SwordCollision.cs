using UnityEngine;
using System.Collections;

public class SwordCollision : MonoBehaviour {
	
	public event System.Action OnTargetDestroyed;
	
	public void OnTriggerEnter(Collider target)
	{
		if(target.gameObject.layer == LayerMask.NameToLayer("BoardTile"))
		{
			NormalTile targetTile = target.GetComponent<NormalTile>();
			
			if(targetTile)
			{
				// Mark that these tiles are going to be destroyed one by one (not gathered in a list and then destroyed in a bulk).
				targetTile.isSingleDestroyed = true;
				
				targetTile.Destroy();
				RaiseOnTargetDestroyed();
			}
		}
    }
	
	public void RaiseOnTargetDestroyed()
	{
		if(OnTargetDestroyed != null)
		{
			OnTargetDestroyed();
		}
	}
}
