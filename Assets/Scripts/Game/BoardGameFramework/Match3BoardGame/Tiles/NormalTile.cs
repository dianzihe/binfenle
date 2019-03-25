using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class NormalTile : Match3Tile 
{	
	/// <summary>
	/// Occurs when a tile finishes it's gravity fall and starts its bounce animation.
	/// </summary>
	public static event System.Action<NormalTile> OnStartTileImpactBounce;
	public static event System.Action<NormalTile, bool> OnStartNormalTileDestroy;
	
	public static event System.Action<BombTile> OnBombTileCreatedFromMatch;
	public static event System.Action<ColorBombTile> OnColorBombTileCreatedFromMatch;
	public static event System.Action<DirectionalDestroyTile> OnDirectionalTileCreatedFromMatch;
	
	/// <summary>
	/// This flag is set by certain special effects that will always destroy tiles one by one (not in bulks). (for example the ColorBomb+ColorBomb effect)
	/// </summary>
	[System.NonSerialized]
	public bool isSingleDestroyed = false;
		
	// Params to control tile bounce effect
	[System.NonSerialized]
	public float fallBounceStiffness = 0.2f;
	[System.NonSerialized]
	public float fallBouncePower = 0.075f;
	[System.NonSerialized]
	public Tweener fallBounceAnimTweener1;
	[System.NonSerialized]
	public Tweener fallBounceAnimTweener2;
	[System.NonSerialized]
	public bool canBounce = false;
	[System.NonSerialized]
	public bool fallBounceAnimEnabled = false;
	
	[System.NonSerialized]
	public NormalTile lastNeighborTile;
	
	public Vector3 freezeOffset = new Vector3(0f, 0f, -4.25f);
	
	protected static WaitForSeconds fallBounceWait;
//	protected static WaitForSeconds fallBounceWait2;
	
	// Params to control tile falling elastic effect
	[System.NonSerialized]
	public float fallElasticStiffness = 0.25f;
	[System.NonSerialized]
	public float fallElasticPower = 0.25f;
	protected bool fallElasticAnimEnabled = false;
	
	// Params to control tile punch effect
	[System.NonSerialized]
	public float punchPower = 0.4f;
	[System.NonSerialized]
	public float punchStiffness = 0.25f;
//	[System.NonSerialized]
//	public float punchbackDelay = 0.05f;
//	protected WaitForSeconds waitPunchbackDelay;
	protected Vector3 punchDirection = Vector3.zero;
	protected bool punchAnimEnabled = false;

	[System.NonSerialized]
	public bool movedByInput = false;
	
	public GameObject prefabFreezeEffect;
	public Material freezeDestroyMaterial;
	
	protected DestroyEffect destroyEffect;
	protected GameObject freezeEffect;
	
		
	public override void InitComponent () {
		base.InitComponent ();

		movedByInput = false;
		
		lastNeighborTile = null;
		
		if (prefabDestroyEffect) {
			destroyEffect = prefabDestroyEffect.GetComponent<DestroyEffect>();
		}
	}
	
	public override RuleEntry DefaultRuleEntry {
		get {
			RuleEntry defaultRule = new RuleEntry();
			defaultRule.RuleTileType = this.GetType();
			
			return defaultRule;
		}
	}
	
	public override bool CanMoveAt(AbstractBoardPiece targetBoardPiece)
	{
		bool canMove = base.CanMoveAt(targetBoardPiece) && !IsFrozen();
//		if ( !canMove ) {
//			Debug.LogWarning(this.name + " tile can't be moved to: " + targetBoardPiece);
//		}

		return canMove;
	}
	
	public override bool CanBeMatched 
	{
		get {
			return base.CanBeMatched && !IsFrozen();
		}
		set {
			base.CanBeMatched = value;
		}
	}

#region Tile Falling Events&Effects
	
	public override void RaiseEventTileStartedActiveGravity () {
		base.RaiseEventTileStartedActiveGravity ();
		
		canBounce = true;
//		Match3BoardPiece curPiece = BoardPiece as Match3BoardPiece;
	}
	
	protected override void TileStoppedMoving(ref Match3BoardPiece startingPiece)
	{
		base.TileStoppedMoving(ref startingPiece);
		
		Match3BoardPiece currentPiece = BoardPiece as Match3BoardPiece;
		// Do the bounce animation only if the actual BoardPiece and the reported current piece where the tile stopped are different.
		if (currentPiece != startingPiece)
		{
			startingPiece = currentPiece;
			
			if ( !fallBounceAnimEnabled ) {
				StartCoroutine(ApplyTileBounceEffect(fallBounceStiffness, fallBouncePower));
			}
		}
	}
	
	public override void RaiseEventTileFinishedActiveGravity (Match3BoardPiece startingBoardPiece) 
	{
		base.RaiseEventTileFinishedActiveGravity(startingBoardPiece);

		IsMoving = false;
		
		if ( !canBounce ) {
			return;
		}
	
		// This tile can't bounce again until it passes at least once through passive gravity again.
		canBounce = false;
		
		if ( (startingBoardPiece != BoardPiece) && !fallBounceAnimEnabled ) {
			StartCoroutine(ApplyTileBounceEffect(fallBounceStiffness, fallBouncePower));
		}
//		StartCoroutine(ApplyTileFallElasticEffect(curPiece, fallElasticStiffness, fallElasticPower));
	}

	public IEnumerator ApplyTileBounceEffect(float bounceStiffness, float bouncePower, float firstBounceBackFactor = 0.4f) {
		if (fallBounceAnimEnabled) {
//			Debug.LogError(name + " Bounce anim already running here!");
			// Don't start the bounce anim if it's already running on this tile
			yield break;
		}
		fallBounceAnimEnabled = true;
	
		if (OnStartTileImpactBounce != null) {
			OnStartTileImpactBounce(this);
		}

		// Make sure the starting local position tile model local position is reset.
		tileModelTransform.localPosition = tileModelLocalPos;
		fallBounceAnimTweener1 = HOTween.To(tileModelTransform, bounceStiffness, new TweenParms()
						 .Prop("localPosition", Vector3.up * bouncePower)
						 .Ease(EaseType.EaseOutExpo)
					  );

		if (NormalTile.fallBounceWait == null) {
		 	NormalTile.fallBounceWait = new WaitForSeconds(bounceStiffness * firstBounceBackFactor);
		}

		yield return NormalTile.fallBounceWait;

		fallBounceAnimTweener2 = HOTween.To(tileModelTransform, bounceStiffness, new TweenParms()
						 .Prop("localPosition", tileModelLocalPos)
						 .Ease(EaseType.EaseOutBounce)
						 .OnComplete(() => fallBounceAnimEnabled = false)
					  );
	}

	public IEnumerator ApplyTileFallElasticEffect(Match3BoardPiece targetPiece, float fallStiffness, float fallPower) {
		fallElasticAnimEnabled = true;		
		// Apply offset to current target piece
		HOTween.To((targetPiece.Tile as Match3Tile).tileModelTransform, fallStiffness, new TweenParms()
					 .Prop("localPosition", Vector3.down * fallPower * 0.75f)
					 .Ease(EaseType.EaseOutSine)
				  );
		
		// Apply offset to bottom neighbor

		HOTween.To((targetPiece.Bottom.Tile as Match3Tile).tileModelTransform, fallStiffness, new TweenParms()
					 .Prop("localPosition", Vector3.down * fallPower)
					 .Ease(EaseType.EaseOutSine)
				  );
		
		// Wait just one frame before overriding the above tweens to make them pull back in the starting position
		yield return null;
		
		// Apply offset comeback to current target piece if we can still do it.
		if (targetPiece != null && targetPiece.Tile != null && !targetPiece.Tile.IsMoving) {
			HOTween.To((targetPiece.Tile as Match3Tile).tileModelTransform, fallStiffness, new TweenParms()
						 .Prop("localPosition", (targetPiece.Tile as Match3Tile).tileModelLocalPos)
						 .Ease(EaseType.EaseOutSine)
					  );
		}
		
		// Apply offset comeback to bottom neighbor piece if we can still do it.
		if (targetPiece.Bottom != null && targetPiece.Bottom.Tile != null && !targetPiece.Bottom.Tile.IsMoving) {
			HOTween.To((targetPiece.Bottom.Tile as Match3Tile).tileModelTransform, fallStiffness, new TweenParms()
						 .Prop("localPosition", (targetPiece.Bottom.Tile as Match3Tile).tileModelLocalPos)
						 .Ease(EaseType.EaseOutSine)
					  );
		}
		
		fallElasticAnimEnabled = false;
	}	
	
	public void ApplyLateUpdatePunchEffect(Vector3 punchDir) {
		// Accumulate the specified punch direction.
		punchDirection += punchDir;
		
		if ( !punchAnimEnabled ) {
			StartCoroutine(LateUpdatePunchEffectCoroutine(punchDir));
		}
	}
		
	protected IEnumerator LateUpdatePunchEffectCoroutine(Vector3 punchDir) {
		punchAnimEnabled = true;
		yield return waitEndOfFrame;
		
		// Apply the punch with the accumulated punch directions during this frame (normalized).
		StartCoroutine(ApplyPunchEffect(punchDirection.normalized, punchStiffness, punchPower));;
	}
		
	protected IEnumerator ApplyPunchEffect(Vector3 punchDir, float punchStiffness, float punchPower) {
		punchAnimEnabled = true;
		Vector3 startPos = tileModelTransform.localPosition;
//		Debug.LogWarning("ApplyPunchEffect: " + name + " -> " + punchDir);
		
		HOTween.To(tileModelTransform, punchStiffness, new TweenParms()
					 .Prop("localPosition", punchDir * punchPower)
					 .Ease(EaseType.EaseOutSine)
				  );

//		yield return waitPunchbackDelay;
		yield return null;
		
		if (cachedTransform != null) {
			HOTween.To(tileModelTransform, punchStiffness, new TweenParms()
						 .Prop("localPosition", startPos)
						 .Ease(EaseType.EaseOutSine)
					  );
		}
		
		punchDirection.x = punchDirection.y = punchDirection.z = 0f;
		punchAnimEnabled = false;
	}
	
	public void ApplyPunchEffectToNeighbors(int xSign, int ySign, int zSign) {
		// Apply punch effect to neighbors at the end of the current frame
		Match3BoardPiece boardPiece = BoardPiece as Match3BoardPiece;
		if (boardPiece != null) {
			int numNeighbors = (int)Match3BoardPiece.LinkType.Count;
			Match3BoardPiece neighbor = null;

			for(int i = 0; i < numNeighbors; i++) {
				neighbor = boardPiece.GetNeighbor( (Match3BoardPiece.LinkType)i );
				if (neighbor != null && neighbor.Tile != null && !neighbor.Tile.IsDestroying) {

					Vector3 punchHeading = neighbor.Tile.LocalPosition - LocalPosition;
					// Some of the tile model axis don't correspond with their parent axis and so the calculated direction must changed.
					punchHeading.x = punchHeading.x * xSign;
					punchHeading.y = punchHeading.y * ySign;
					punchHeading.z = punchHeading.z * zSign;
					
					// Apply the punch effect at the end of the current frame
					(neighbor.Tile as NormalTile).ApplyLateUpdatePunchEffect(punchHeading);
				}
			}
		}
	}
#endregion
	
	protected override void TileDestroy(bool useEffect) 
	{
		if(useEffect)
		{
			CheckForSpawnPatterns();
		}
		
		if (useDestroyEffect && useEffect) {
			Transform effectInstance = SpawnDestroyEffect(Vector3.zero);
			
			if (freezeEffect) {
				Destroy(freezeEffect);
				effectInstance.GetComponent<TileDestroyEffect>().UpdateMaterial(freezeDestroyMaterial);
			}
			else {
				effectInstance.GetComponent<TileDestroyEffect>().UpdateMaterial(TileColor);
			}
			
			StartCoroutine(DelayedDestroy());
			
			if (OnStartNormalTileDestroy != null) {
				OnStartNormalTileDestroy(this, isSingleDestroyed);
			}
		} 
		else {
			base.TileDestroy(false);
		}
	}
	
	protected Transform SpawnDestroyEffect(Vector3 offset) {
		return SpawnDestroyEffect(WorldPosition, offset);
	}
	
	protected Transform SpawnDestroyEffect(Vector3 origin, Vector3 offset) {
		Transform effectInstance = (Instantiate(prefabDestroyEffect) as GameObject).transform;
		effectInstance.position = origin + offset;
		Destroy(effectInstance.gameObject, destroyEffect.lifeTime);
		
		return effectInstance;
	}
	
	protected bool CheckForBomb()
	{
		int sumH = matchCount[(int)TileMatchDirection.Left] + matchCount[(int)TileMatchDirection.Right];
		int sumV = matchCount[(int)TileMatchDirection.Top] + matchCount[(int)TileMatchDirection.Bottom];
		if (sumH >= 2 && sumH < 4 && sumV >= 2 && sumV < 4)
		{
			BoardPiece.Tile = (BoardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(BoardPiece.BoardPosition.row,
				BoardPiece.BoardPosition.col, typeof(BombTile), TileColorType.None);
			(BoardPiece.Tile as Match3Tile).TileColor = TileColor;
			(BoardPiece.Tile as BombTile).UpdateMaterial();
			
			(BoardPiece.Tile as NormalTile).movedByInput = movedByInput;
			
			if (OnBombTileCreatedFromMatch != null) {
				OnBombTileCreatedFromMatch(BoardPiece.Tile as BombTile);
			}
			return true;
		}
		
		return false;
	}
	
	protected bool CheckForSpawnPatterns()
	{
		bool bombResult = CheckForBomb();
		bool colorBombResult = CheckForColorBomb();
		bool directionalResult = CheckForColorDirectionalDestroy();
		
		return bombResult || colorBombResult || directionalResult;
	}
	
	protected bool CheckForColorBomb()
	{
		if ((matchCount[(int)TileMatchDirection.Left] == 2 && matchCount[(int)TileMatchDirection.Right] >= 2) ||
			(matchCount[(int)TileMatchDirection.Top] == 2 && matchCount[(int)TileMatchDirection.Bottom] >= 2))
		{
			BoardPiece.Tile = (BoardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(BoardPiece.BoardPosition.row,
				BoardPiece.BoardPosition.col, typeof(ColorBombTile), TileColorType.None);
			BoardPiece.Tile.StartCoroutine((BoardPiece.Tile as ColorBombTile).StartIdleAnim());
			
			if (OnColorBombTileCreatedFromMatch != null) {
				OnColorBombTileCreatedFromMatch(BoardPiece.Tile as ColorBombTile);
			}
			
			return true;
		}
		
		return false;
	}
	
	protected bool CheckForColorDirectionalDestroy()
	{
		int sumH = matchCount[(int)TileMatchDirection.Left] + matchCount[(int)TileMatchDirection.Right];
		int sumV = matchCount[(int)TileMatchDirection.Top] + matchCount[(int)TileMatchDirection.Bottom];

		if (sumH == 3 && sumV < 2 && (matchCount[(int)TileMatchDirection.Left] == 2 || matchCount[(int)TileMatchDirection.Right] == 2)) {
			NormalTile tileLeft = (BoardPiece as Match3BoardPiece).Left.Tile as NormalTile;
			NormalTile tileRight = (BoardPiece as Match3BoardPiece).Right.Tile as NormalTile;
			
			if (movedByInput || ((tileLeft == null || !tileLeft.movedByInput) && (tileRight == null || !tileRight.movedByInput))) {
				movedByInput = true;
				StartCoroutine(SpawnDirectionalDestroy(typeof(ColumnDestroyTile)));
				return true;
			}
		}
		
		if (sumV == 3 && sumH < 2 && (matchCount[(int)TileMatchDirection.Top] == 2 || matchCount[(int)TileMatchDirection.Bottom] == 2)) {
			NormalTile tileTop = (BoardPiece as Match3BoardPiece).Top.Tile as NormalTile;
			NormalTile tileBottom = (BoardPiece as Match3BoardPiece).Bottom.Tile as NormalTile;
			
			if (movedByInput || ((tileTop == null || !tileTop.movedByInput) && (tileBottom == null || !tileBottom.movedByInput))) {
				movedByInput = true;
				StartCoroutine(SpawnDirectionalDestroy(typeof(RowDestroyTile)));
				return true;
			}
		}
		
		return false;
	}
	
	IEnumerator SpawnDirectionalDestroy(System.Type destroyType)
	{
		yield return waitEndOfFrame;

		BoardPiece.Tile = (BoardRenderer as Match3BoardRenderer).SpawnSpecificTileAt(BoardPiece.BoardPosition.row,
			BoardPiece.BoardPosition.col, destroyType, TileColorType.None);
		(BoardPiece.Tile as Match3Tile).TileColor = TileColor;
		(BoardPiece.Tile as DirectionalDestroyTile).UpdateMaterial();
		
		if (OnDirectionalTileCreatedFromMatch != null) {
			OnDirectionalTileCreatedFromMatch(BoardPiece.Tile as DirectionalDestroyTile);
		}
	}
	
	public virtual bool Freeze()
	{
		if (prefabFreezeEffect != null && freezeEffect == null && !IsDestroying) {
			IsUserMoveable = false;
			CanBeMatched = false;
			
			Transform effectInstance = (Instantiate(prefabFreezeEffect) as GameObject).transform;
			Vector3 freezeLocalScale = effectInstance.localScale;
			
			effectInstance.parent = tileModelTransform;
			effectInstance.localPosition = freezeOffset;
			effectInstance.localScale = freezeLocalScale;
			freezeEffect = effectInstance.gameObject;
			
			return true;
		}
		return false;
	}
	
	public bool IsFrozen() {
		return freezeEffect != null;
	}
	
	public override void RaiseEventTileSwitchAnimBegan(Match3Tile neighborTile) 
	{
		base.RaiseEventTileSwitchAnimBegan(neighborTile);
		lastNeighborTile = neighborTile as NormalTile;
		
		movedByInput = true;
	}
	
	public override void RaiseEventTileSwitchAnimEnded (AbstractTile neighborTile)
	{
		base.RaiseEventTileSwitchAnimEnded (neighborTile);
		
		if ( !IsMatched ) {
			movedByInput = false;
		}
	}
	
	protected IEnumerator DelayedDestroy()
	{	
		tileModelTransform.gameObject.SetActive(false);
		return DelayedDestroy(destroyEffect.destroyTileTime);
	}
	
	private void BaseFileDestroy(bool useEffect) {
		base.TileDestroy(useEffect);
	}
	
	protected IEnumerator DelayedDestroy(float delayTime)
	{
//		ApplyPunchEffectToNeighbors(-1, 1, 1);
		yield return new WaitForSeconds(delayTime);
		
		BaseFileDestroy(false);
	}
	
	protected IEnumerator DelayedSetActive(float delayTime) {
		yield return new WaitForSeconds(delayTime);
		tileModelTransform.gameObject.SetActive(false);
	}
}
