using UnityEngine;
using System.Collections;

public class TimeBombTile : BombTile
{
	public float time = 3f;
		
	public override void InitComponent () {
		base.InitComponent();
		
		StartCoroutine(Timer());
	}
	
	protected IEnumerator Timer()
	{
		float timer = 0f;
		GetComponent<Animation>().Play();
		
		while (timer < time) {
			yield return null;
			timer += Time.deltaTime;
		}
		
		Destroy();
		
		Match3BoardGameLogic.Instance.TryCheckStableBoard();
	}
	
	public override void UpdateMaterial ()
	{
		tileModelRenderer.material = coloredMaterials[(int)TileColor - 1];
		//MALEFICENT NEW
		backLightRenderer.material = backLightMaterials[(int)TileColor - 1];
		breakEffectRenderer.material = breakEffectMaterials[(int)TileColor - 1];
	}
}

