using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

/// <summary>
/// EaterTile
/// 
/// This tile is not moveable and can only be destroyed aftear eating an specific amount of tiles of specific color(s)
/// </summary>
public class EaterTile : NormalTile
{
	public static event System.Action<EaterTile> OnEaterTileInit;

	// color selection
	public List<TileColorType> eatColorsList;
	public GenericColorType genericEatColor;
	TileColorType eatColor;
	public int tilesToEat = 0;
	public ColorSelectionMethod selectionMethod;
	
	private bool finished = false;
	private Match3BoardPiece topBoardPiece;
	private float tileEatAnimDelay = 0.2f;
	private int tilesEaten = 0;
	float minPercentage = 0.5f;
	bool eating = false;

	protected override void Awake()
	{
		base.Awake();

		BoardShuffleController.OnBoardShuffleFinished += HandleOnBoardShuffleFinished;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		BoardShuffleController.OnBoardShuffleFinished -= HandleOnBoardShuffleFinished;
	}

	void HandleOnBoardShuffleFinished ()
	{
		StartCoroutine(CheckTopTileColor()); 
	}

	public override void InitComponent()
	{
		base.InitComponent();

		// for the winning condition
		if(OnEaterTileInit != null)
		{
			OnEaterTileInit(this);
		}
		eating = false;

		// init color
		if(selectionMethod == ColorSelectionMethod.Generic)
		{
			eatColor =  RuleEntry.genericColors[(int)genericEatColor];
		}
		else if(eatColorsList.Count > 0)
			eatColor = eatColorsList[0];


		transform.Find("tile_eater_liquid_01").GetComponent<Renderer>().material.SetColor("_Color",GetBottleColor());
		//transform.Find("tile_eater_liquid_01").GetComponent<Renderer>().material.SetFloat("_Percentage",minPercentage);
		float currentPercentage = (float)(tilesEaten +1)/(float)tilesToEat;
		float offsetPercentage = minPercentage + currentPercentage*(1-minPercentage);
		transform.Find("tile_eater_liquid_01").GetComponent<Renderer>().material.SetFloat("_Percentage",offsetPercentage);
	}

	Color GetBottleColor()
	{
		Color bottleColor = Color.red;

		switch(eatColor)
		{
		case TileColorType.Red:
			bottleColor = new Color(0.6f,0f,0f);
			break;
		case TileColorType.Green:
			bottleColor = new Color(0.08f,0.43f,0.01f);
			break;
		case TileColorType.Blue:
			bottleColor = new Color(0.01f,0.2f,0.63f);
			break;
		case TileColorType.Yellow:
			bottleColor = new Color(0.6f,0.52f,0f);
			break;
		case TileColorType.Purple:
			bottleColor = new Color(0.5f, 0.08f, 0.6f);
			break;
		case TileColorType.Orange:
			bottleColor = new Color(0.54f, 0.54f, 0.54f);
			break;

		}

		return bottleColor;
	}

	protected override void Start ()
	{	
		base.Start();

		// inits itself regarding the boardpiece on the top
		topBoardPiece = (BoardPiece as Match3BoardPiece).Top;
		if(topBoardPiece != null)
		{
			// top tile will be checked every time it changes...
			topBoardPiece.OnTileChanged += OnTopTileChanged;
			StartCoroutine(CheckTopTileColor());
		}
	}

	protected void OnTopTileChanged(AbstractBoardPiece sender, AbstractTile tile)
	{
		StartCoroutine(CheckTopTileColor()); 
	}
	

	// this function checks the top tile, if it's the same color and it's not locked, it will be eatable
	private IEnumerator CheckTopTileColor()
	{
		if(!eating && !finished && !BoardShuffleController.Instance.IsBoardReshuffling)
		{
			Match3Tile topTile = topBoardPiece.Tile as Match3Tile;

			if(topTile != null)
			{

				while(topTile == null || topTile.IsTileSwitching || topTile.IsMatched || topTile.IsDestroying || topTile.IsMoving || eating)
				{
					yield return null;

					// refresh top tile
					topTile = topBoardPiece.Tile as Match3Tile;
				}

				if(!(topTile is LockedTile) && topTile.TileColor == eatColor && !finished)
				{
					eating = true;
					Eat(topTile);
				}
			}
		}

		yield return null;
	}

	protected void Eat(Match3Tile toptile)
	{
		if(!finished) {
			tilesEaten++;
			finished = tilesEaten >= tilesToEat;
			
			Transform topTileTransform = toptile.transform;
			toptile.DisableTileLogic();
			toptile.IsDestructible = false;
			(toptile.BoardPiece as Match3BoardPiece).BlockCount++;

			SoundManager.Instance.Play("tile_eater_sfx");


			//Eat tween
			HOTween.To(topTileTransform, tileEatAnimDelay, 
			           new TweenParms().Prop("position", cachedTransform.position)
			           .Ease(EaseType.EaseInSine)
			           .OnComplete(OnEatTweenComplete, toptile)
			           );
		}
	}
	
	protected void OnEatTweenComplete(TweenEvent eventData)
	{
		Match3Tile topTile = (Match3Tile)eventData.parms[0];
		eating = false;
		float currentPercentage = (float)(tilesEaten +1)/(float)tilesToEat;
		float offsetPercentage = minPercentage + currentPercentage*(1-minPercentage);
		if(!finished)
			transform.Find("tile_eater_liquid_01").GetComponent<Renderer>().material.SetFloat("_Percentage",offsetPercentage);

		if(topTile != null) {
			(topTile.BoardPiece as Match3BoardPiece).BlockCount--;
			topTile.IsDestructible = true;
			//topTile.IsDestructible = true;
			topTile.Destroy();
		}

		// cannot eat more tiles, need to be destroyed
		if(finished)
		{
			AddScore();
			topBoardPiece.OnTileChanged -= OnTopTileChanged;
			IsDestructible = true;
			Destroy();
		}else {
			StartCoroutine(CheckTopTileColor());
		}
	}
	
	protected override void TileDestroy(bool useEffect)
	{
		base.TileDestroy(false);
	}


}
