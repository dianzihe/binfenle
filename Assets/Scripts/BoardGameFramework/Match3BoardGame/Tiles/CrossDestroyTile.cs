using UnityEngine;
using System.Collections;

public class CrossDestroyTile : DirectionalDestroyTile
{
	public float destroyDelay = 0f;
	
	protected override void Awake ()
	{
		base.Awake();
		
		direction = DirectionalDestroyTile.Direction.Cross;
		
		StartCoroutine(Timer());
	}
	
	public override void InitComponent ()
	{
		base.InitComponent ();
		parentDestroyAnimationName = "effect_winterchill2_cross";
	}
	
	public override void UpdateMaterial ()
	{
		//TODO: Clean up on aisle 5
		//Old implementation required each arrow to take on a coresponding material
		//New effect no longer requires this
		
//		tileModelTransform.Find("tile_directional_new1").renderer.material = coloredMaterials[(int)TileColor - 1];
//		tileModelTransform.Find("tile_directional_new2").renderer.material = coloredMaterials[(int)TileColor - 1];
	}
	
	//Timer coroutine
	protected IEnumerator Timer()
	{
		yield return new WaitForSeconds(destroyDelay);
		
		GetComponent<Animation>().Play();
		Destroy();
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
}

