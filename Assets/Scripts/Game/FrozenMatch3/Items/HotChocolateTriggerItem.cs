using UnityEngine;
using System.Collections;

public class HotChocolateTriggerItem : MonoBehaviour
{
	public static event System.Action<HotChocolateTriggerItem, Match3Tile> OnTargetTriggeredEvent;
	public event System.Action<HotChocolateTriggerItem> OnTriggerFinishedMove;
	
	[System.NonSerialized]
	public Transform cachedEffectTransform;
	[System.NonSerialized]
	public ParticleSystem cachedEffectParticleSystem;
	
	protected float lerpSpeed = 2.3f;
	protected float lerpAmount = 0.0f;
	
	public AnimationCurve dampCurve;
	
	public Vector3 direction = Vector3.down;
	
	[System.NonSerialized]
	public int numTilesTraveled = 1;

	protected float collisionDamp = 0.0f;
	
	protected float initialWaitTime = 0f;
	protected float postEffectWaitTime = 1f;
	
	[System.NonSerialized]
	public Transform cachedTransform;
	
	private static int boardPieceLayerId = -1;
	private static int boardTileLayerId = -1;
	
	void Awake()
	{
		cachedTransform = transform;
		cachedTransform.position += Vector3.up * Match3BoardRenderer.halfVertTileDistance - Vector3.up * 0.1f;
		
		// One time cache board piece and board tile layer ids.
		if (boardPieceLayerId < 0) {
			boardPieceLayerId = LayerMask.NameToLayer("BoardPiece");
		}
		
		if (boardTileLayerId < 0) {
			boardTileLayerId = LayerMask.NameToLayer("BoardTile");
		}
	}
	
	public void SetEffect(Transform effectTransform, ParticleSystem effectParticleSystem)
	{
		cachedEffectTransform = effectTransform;
		cachedEffectParticleSystem = effectParticleSystem;
	}
	
	public IEnumerator Move() 
	{
		cachedEffectTransform.GetComponent<ParticleSystem>().enableEmission = true;
		cachedEffectParticleSystem.enableEmission = true;
		
		yield return new WaitForSeconds(initialWaitTime);
		
		Vector3 targetPosition = cachedTransform.position + direction * numTilesTraveled * Match3BoardRenderer.vertTileDistance + Vector3.down * 0.3f;
		Vector3 initialPosition = cachedTransform.position;
		
		float stopEmmisionThreshold = 1f / numTilesTraveled * (numTilesTraveled - 0.2f);
		float lerpDelta = lerpSpeed / numTilesTraveled;
		
		while(lerpAmount <= 1.0f)
		{	
			lerpAmount = Mathf.Clamp01(lerpAmount + lerpDelta * Time.deltaTime * (1f - collisionDamp));
			cachedTransform.position = Vector3.Lerp(initialPosition, targetPosition, lerpAmount);
			
			if(lerpAmount > stopEmmisionThreshold)
			{
				cachedEffectParticleSystem.enableEmission = false;
				cachedEffectTransform.GetComponent<ParticleSystem>().enableEmission = false;
			}
			yield return null;
			
			if (lerpAmount == 1f)
			{
				break;
			}
		}
		
		yield return new WaitForSeconds(postEffectWaitTime);
		
		RaiseOnTriggerFinishedMoveEvent();
	}
	
	void RaiseOnTargetTriggeredEvent(HotChocolateTriggerItem hotChocolateTriggerItem, Match3Tile tile)
	{
		if(OnTargetTriggeredEvent != null)
		{
			OnTargetTriggeredEvent(hotChocolateTriggerItem, tile );
		}
	}
	
	void RaiseOnTriggerFinishedMoveEvent()
	{
		if(OnTriggerFinishedMove != null)
		{
			OnTriggerFinishedMove(this);
		}
	}
	
	void OnTriggerEnter(Collider collider)
	{
		RaiseOnTargetTriggeredEvent(this, collider.GetComponent<Match3Tile>());
		
		Match3Tile tile = null;
		if (collider.gameObject.layer == boardTileLayerId) {
			tile = collider.GetComponent<Match3Tile>();
		}
		
		Match3BoardPiece boardPiece = null;
		if (collider.gameObject.layer == boardPieceLayerId) {
			boardPiece = collider.GetComponent<Match3BoardPiece>();
		}
		
		if (tile)
		{
			StartCoroutine(ActionOnTriggerEnter(tile));
		}
		
		if(boardPiece)
		{
			StartCoroutine(ActionOnTriggerEnter(boardPiece));
		}
	}
	
	protected IEnumerator ActionOnTriggerEnter(Object collidedObject)
	{	
		Match3Tile tile = null;
		Match3BoardPiece boardPiece = null;
		
		if(collidedObject is Match3Tile)
		{
			tile = collidedObject as Match3Tile;
			boardPiece = tile.BoardPiece as Match3BoardPiece;
		}
		
		if(collidedObject is Match3BoardPiece)
		{
			boardPiece = collidedObject as Match3BoardPiece;
			tile = boardPiece.Tile as Match3Tile;
		}
		
		bool applyDamp = false;
		
//		if(IsValidTargetTile(tile))
//		{
//			applyDamp = true;
//		}
		
		float currentLerpAmount = lerpAmount;
		
		while((lerpAmount - currentLerpAmount) < (1f / numTilesTraveled) && lerpAmount != 1f)
		{
//				Debug.LogWarning("lerpAmount: " + lerpAmount);
			
			if(applyDamp)
			{
				collisionDamp = dampCurve.Evaluate((lerpAmount - currentLerpAmount) / (1f / numTilesTraveled));
			}
//				Debug.LogWarning("Collision damp: " + collisionDamp);
			yield return null;
		}
		
		if(boardPiece is LayeredBoardPiece)
		{
			(boardPiece as LayeredBoardPiece).NumLayers = 0;
		}
		
		if (tile)
		{	
//			Debug.LogWarning("Tile Destroyed!");
			
			if (tile is SnowTile)
			{
				(tile as SnowTile).ChocoDestroy();
			}
			
			if(tile is LockedTile)
			{
				(tile as LockedTile).spawnUnlockedTile = false;
			}
			
			//If SunTile is hit by hot chocolate u set its state to the one prior to its explosion state
			//What this means is when the hot chocolate will call Destroy() the sunTile will be set to its explosion state.
			if(tile is SunTile)
			{
				tile.DisableTileLogic();
				(tile as SunTile).TargetSize = SunTile.STATE_EXPLODE-1;
			}
			
			tile.Destroy();
		}
		
		collisionDamp = 0f;
	}

	public bool IsValidTargetTile(Match3Tile tile)
	{
		if(tile && tile is SnowTile || tile is FreezerTile || tile is LockedTile)
		{
			return true;
		}
		
		return false;
	}

/// <summary>
//	Used in debugging.
/// </summary>
	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(cachedTransform.position, cachedTransform.localScale);
	}
}
