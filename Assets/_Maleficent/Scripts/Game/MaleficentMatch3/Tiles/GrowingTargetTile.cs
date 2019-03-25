using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// TODO:
/// GrowingTargetTile
/// 
/// If you do a match next to it, it grows
/// </summary>
public class GrowingTargetTile : NormalTile {
	
	public static event System.Action<GrowingTargetTile> OnGrowTargetTileInit;
	public static event System.Action<GrowingTargetTile> OnGrowTargetWinCondition;

	public int targetSize = 1;

	protected int maxTargetSize = 5;
	protected Match3BoardPiece boardPieceIterator;
	protected bool isGrowing = false;
	//protected bool hasGrown = false;
	protected SpriteRenderer tileSpriteRenderer;
	protected Sprite[] spriteArray;
	protected string resourcePath = "Game/Materials/Tiles/GrowingTargetTile/thorns_atlas";
	protected Animator cachedAnimator;
	bool started = false;
	
	protected override void Awake ()
	{
		base.Awake ();

		spriteArray = new Sprite[maxTargetSize];

		cachedAnimator = GetComponent<Animator>() as Animator;
		//cachedAnimator.enabled = false;

		Sprite[] sprites = Resources.LoadAll<Sprite>(resourcePath);
		
		for(int i = 0; i < maxTargetSize; i++)
		{
			spriteArray[i] = sprites[i];
		}
	}
	
	public override void InitComponent () {
		base.InitComponent();
		
		TileColor = TileColorType.None;
		tileSpriteRenderer = tileModelRenderer as SpriteRenderer;
		TargetSize = targetSize;
		
		if (OnGrowTargetTileInit != null) {
			OnGrowTargetTileInit(this);
		}
		started = true;
	}

	protected int TargetSize {
		get 
		{
			return targetSize;
		}

		set
		{
			targetSize = value;

			int spriteIdx = targetSize - 1;
			
			if(spriteIdx == maxTargetSize)
			{
				if(started)
					SoundManager.Instance.Play("grow_targets_complete_sfx");
				cachedAnimator.SetTrigger("state" + targetSize);
				RegisterNeighborBoardPieces(false);
				
				if (OnGrowTargetWinCondition != null)
				{
					OnGrowTargetWinCondition(this);
				}
			}else {
				if(started)
					SoundManager.Instance.Play("grow_targets_sfx");
				cachedAnimator.SetTrigger("state" + targetSize);
				//tileSpriteRenderer.sprite = spriteArray[spriteIdx];
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

	public void OnNeighborDestroy(AbstractBoardPiece sender, AbstractTile neighbor)
	{	
		if(neighbor != null)
		{
		//	if (!isGrowing && !hasGrown)
			{
				TargetSize++;
			}
		}
	}

	public override Object Thumbnail ()
	{
		return transform.Find("Model").GetComponent<SpriteRenderer>().sprite;
	}
}
