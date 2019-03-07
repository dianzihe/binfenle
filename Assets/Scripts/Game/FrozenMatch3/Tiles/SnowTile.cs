using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TODO:
/// SnowTile
/// 
/// You can’t move it, you have to make a group match adjacent to it to destroy the snow tile. It doesn’t have a color.
/// It can’t be created, it appears on the board by initial design.
/// </summary>
public class SnowTile : NormalTile {

	public static event System.Action<SnowTile> OnSnowTileInit;
	public static event System.Action<SnowTile> OnSnowWinDestroyCondition;
	public static event System.Action<SnowTile> OnSnowTileLayerBeginDestroy;
	
	//Number of snow layers => number of matches that need to be acomplished in adjacency to this tile for it to destroy.
	public int numberOfSnowLayers = 1;
	
	protected int oldNumberOfSnowLayers;
	
	public GameObject snowDestroyPrefab;
	
	protected Match3BoardPiece boardPieceIterator;
	
	public string[] animationClipNames;
	
	//Material array (populated by loading specific mats from resources).
	protected Material[] materialArray;
	protected string resourcePath = "Game/Materials/SnowTile/tile_snowcube";
	protected int materialArraySize = 5;
	
	protected override void Awake ()
	{
		base.Awake ();
		
		materialArray = new Material[materialArraySize];
		
		for(int i = 0; i < materialArraySize; i++)
		{
			materialArray[i] = Resources.Load(resourcePath + (i+1)) as Material;
		}
	}
	
	public override void InitComponent () {
		base.InitComponent();
		
		TileColor = TileColorType.None;
		
		tileModelRenderer.material = materialArray[numberOfSnowLayers - 1];
		
		if (OnSnowTileInit != null) {
			OnSnowTileInit(this);
		}
	}
	
	protected int NumLayers {
		get 
		{
			return numberOfSnowLayers;
		}
		
		set
		{
			oldNumberOfSnowLayers = numberOfSnowLayers;
			numberOfSnowLayers = value;
			
			if(numberOfSnowLayers == 0)
			{
				RegisterNeighborBoardPieces(false);
			}
			
			if(numberOfSnowLayers < oldNumberOfSnowLayers && numberOfSnowLayers >= 0)
			{
				//Raise OnSnowTileLayerDestroyed Event
				if ( numberOfSnowLayers > 0 && OnSnowWinDestroyCondition != null)
				{
					OnSnowWinDestroyCondition(this);
				}
				
				//Raise OnSnowTileLayerBeginDestroy Event
				if (OnSnowTileLayerBeginDestroy != null)
				{
					OnSnowTileLayerBeginDestroy(this);
				}
				
				GetComponent<Animation>().PlayQueued(animationClipNames[0]);
			}
		}
	}
	
	public override void InitAfterAttachedToBoard ()
	{
		base.InitAfterAttachedToBoard ();
		
		RegisterNeighborBoardPieces(true);
	}
	
	public override RuleEntry DefaultRuleEntry {
		get {
			RuleEntry defaultRule = new RuleEntry();
			defaultRule.RuleTileType = GetType();
			defaultRule.randomColor = false;
			defaultRule.ColorSelection = ColorSelectionMethod.ColorBased;
			defaultRule.RuleTileColor = TileColorType.None;
			
			return defaultRule;
		}
	}

	protected override bool PreDestroy() {
		if (numberOfSnowLayers > 0) {
			
			NumLayers--;
			
			return false;
		}
		else {
			return base.PreDestroy();
		}
	}
	
	public override TileColorType TileColor {
		get {
			return base.TileColor;
		}
		set {
			base.TileColor = TileColorType.None;
		}
	}
	
	private void RegisterNeighborBoardPieces(bool subscribe) {
		Match3BoardPiece match3BoardPiece = BoardPiece as Match3BoardPiece;
		
		for(int i = 0; i < match3BoardPiece.neighbors.Length; i+=2) {
			
			boardPieceIterator = match3BoardPiece.neighbors[i];
			
			if(boardPieceIterator == null) {
				continue;
			}
		
			if(subscribe) {
				boardPieceIterator.OnTileDestroyed += OnNeighborDestroy;
			} else {
				boardPieceIterator.OnTileDestroyed -= OnNeighborDestroy;
			}
		}
	}
	
	//Animation Event raised on the first keyframe of the destroy anim.
	protected IEnumerator PlayDestroyAnimation()
	{		
		Transform effectInstance = (Instantiate(snowDestroyPrefab) as GameObject).transform;
		effectInstance.position = WorldPosition + new Vector3(0f, 0f, -2f);
		destroyEffect = effectInstance.GetComponent<DestroyEffect>();
		SnowDestroyEffect snowDestroyEffect = destroyEffect as SnowDestroyEffect;
		Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
		
		if (numberOfSnowLayers > 1)
		{
			snowDestroyEffect.cachedModelRenderer.GetComponent<Renderer>().material = materialArray[numberOfSnowLayers-1];
			effectInstance.GetComponent<Animation>().Play();
		}
		else
		{
			snowDestroyEffect.HideRenderer();
		}

		if (numberOfSnowLayers == 0) {
			tileModelRenderer.enabled = false;
		}
		
		yield return new WaitForSeconds(GetComponent<Animation>()[animationClipNames[0]].length);
				
		if (numberOfSnowLayers == 0)
		{
			Destroy();
			yield break;
		}
		
		if(numberOfSnowLayers > 0)
		{
			tileModelRenderer.material = materialArray[numberOfSnowLayers-1];
			GetComponent<Animation>().Play(animationClipNames[1]);
		}
	}
	
	public void OnNeighborDestroy(AbstractBoardPiece sender, AbstractTile neighbor)
	{	
		if(neighbor && (neighbor as NormalTile).IsMatched)
		{
			NumLayers--;
		}
	}
	
	protected override void TileDestroy (bool useEffect)
	{
		base.TileDestroy(false);
	}
	
	
	public override bool Freeze()
	{
		if (!IsDestroying) {
			return true;
		}
		
		return false;
	}
	
	public void ChocoDestroy()
	{
		for(int index = numberOfSnowLayers; index > 1; index--)
		{
			//Raise OnSnowTileLayerDestroyed Event
			if ( numberOfSnowLayers > 0 && OnSnowWinDestroyCondition != null)
			{
				OnSnowWinDestroyCondition(this);
			}
		}
		numberOfSnowLayers = 1;
	}
}
