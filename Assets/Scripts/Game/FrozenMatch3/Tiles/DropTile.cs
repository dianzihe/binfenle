using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class DropTile : NormalTile {
	
	public int dropTileID = 1;
	
	public float tileDropAnimDelay = 0.2f;
	
	public delegate void DropTileHandler(DropTile sender);
	
	public static event DropTileHandler OnDropTileInit;
	public static event DropTileHandler OnDropTileDropped;

	//Is this dropTile involved in a switch anim atm ?
	public bool isSwitching = false;
	public bool isDropping = false;
	
	protected override void TileDestroy(bool useEffect)
	{
		base.TileDestroy(false);
	}
	
	public override void InitComponent ()
	{
		base.InitComponent ();
		
		if(OnDropTileInit != null) {
			OnDropTileInit(this);
		}
	}
	
	
	public override RuleEntry DefaultRuleEntry {
		get {
			RuleEntry defaultRule = new RuleEntry();
			defaultRule.RuleTileType = GetType();
			defaultRule.randomColor = false;
			defaultRule.ColorSelection = ColorSelectionMethod.ColorBased;
			defaultRule.RuleTileColor = TileColor;
			
			return defaultRule;
		}
	}
	
	public override void RaiseEventTileSwitchAnimBegan (Match3Tile neighborTile)
	{
		base.RaiseEventTileSwitchAnimBegan (neighborTile);
		isSwitching = true;
	}
	
	public override void RaiseEventSwitchSuccess (Match3Tile neighborTile)
	{
		base.RaiseEventSwitchSuccess (neighborTile);
		
		//If this boardpiece is a dropperBehaviour
		if (BoardPiece.GetComponent<DropperBehaviour>() != null && !isDropping)
		{
			StartCoroutine(Drop ());
		}
	}
	
	public override void RaiseEventTileSwitchAnimEnded (AbstractTile neighborTile)
	{
		base.RaiseEventTileSwitchAnimEnded (neighborTile);
		isSwitching = false;
	}
	
	public IEnumerator Drop()
	{
		isDropping = true;
		
		while(IsMoving)
		{
			yield return null;
		}
	
		GravityUpdateEnabled = true;
		tileModelTransform.gameObject.layer = LayerMask.NameToLayer("DropTile");
		
		AddScore();
				
		//Drop tween
		HOTween.To(cachedTransform, tileDropAnimDelay, 
   				  new TweenParms().Prop("localPosition", cachedTransform.localPosition +
			      Vector3.down * (BoardRenderer as Match3BoardRenderer).verticalTileDistance * cachedTransform.localScale.x)
				  .Ease(EaseType.EaseInSine)
				  .OnComplete(OnDropTweenComplete)
   				 );
		
		//Detach tile 
		BoardPiece.Tile = null;
		
		if (OnDropTileDropped != null) {
			OnDropTileDropped(this);
		}
	}
	
	protected void OnDropTweenComplete()
	{
		IsDestructible = true;
		Destroy();
	}
}
