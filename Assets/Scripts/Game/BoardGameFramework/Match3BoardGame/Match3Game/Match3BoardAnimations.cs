using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;


/// <summary>
/// Board animations encapsulates methods(coroutines) to do various board tiles and board pieces animations.
/// Also provides callbacks to allow game logic to update correctly
/// Ex. Moving tiles from a position to another position.
/// </summary>
public class Match3BoardAnimations : AbstractBoardAnimations {
	public delegate void DelegateTilesSwitchAnimFinished(AbstractBoardAnimations sender, AbstractTile src, AbstractTile dest);
	public delegate void DelegateIntroAnimFinished(AbstractBoardAnimations sender);
	
	protected event DelegateTilesSwitchAnimFinished onTilesSwitchAnimFinished;
	public event DelegateIntroAnimFinished OnIntroAnimFinished;
	
	protected float tileSwitchAnimDelay = 0.2f;
	
	
	public GameObjectEvent OnBoardIntroAnimFinished = new GameObjectEvent(null, "SendFsmEvent", "OnBoardIntroAnimFinished");

	
	public event DelegateTilesSwitchAnimFinished OnTilesSwitchAnimFinished {
		add {
			onTilesSwitchAnimFinished += value;
		}
		remove {
			onTilesSwitchAnimFinished -= value;
		}
	}
		
	public void RaiseEventTilesSwitchAnimFinished(bool canRaiseGlobalSwitchEvent, AbstractBoardAnimations sender, AbstractTile src, AbstractTile dst) {
		// These 2 tiles aren't animating anymore.
		src.IsMoving = false;
		dst.IsMoving = false;
		
		(src as Match3Tile).GravityEnabled = true;
		(dst as Match3Tile).GravityEnabled = true;
		
		if (canRaiseGlobalSwitchEvent && onTilesSwitchAnimFinished != null) {
			onTilesSwitchAnimFinished(sender, src, dst);
		}
	}
	
	public void SwitchTilesAnim(bool canRaiseGlobalSwitchEvent, Match3Tile src, Match3Tile dst, System.Action<Match3BoardAnimations, Match3Tile, Match3Tile> onAnimFinished = null) {
//		Debug.Log("[" + this.GetType().Name + "] SwitchTilesAnim " + src.name + " -> " + dst.name);
		src.IsMoving = true;
		dst.IsMoving = true;
		
		src.GravityEnabled = false;
		dst.GravityEnabled = false;
		
		//TODO: use a Tweener object pool and a TweenParams object pool for performance reasons
		HOTween.To(src.cachedTransform, tileSwitchAnimDelay, 
											new TweenParms().Prop("localPosition", dst.BoardPiece.LocalPosition)
											.Ease(EaseType.EaseInSine)
										 );
		Tweener dstToSrcAnim = HOTween.To(dst.cachedTransform, tileSwitchAnimDelay, 
											new TweenParms().Prop("localPosition", src.BoardPiece.LocalPosition)
											.Ease(EaseType.EaseInSine)
										 );

		dstToSrcAnim.ApplyCallback(CallbackType.OnComplete, () =>
		{
			// Call a single registered custom method when the tile switch animation finishes. (without registering it for future callbacks)
			if (onAnimFinished != null) {
				onAnimFinished(this, src, dst);
			} 
				
			// Notify any registered methods of this event.
			RaiseEventTilesSwitchAnimFinished(canRaiseGlobalSwitchEvent, this, src, dst);
			
			src.RaiseEventTileSwitchAnimEnded(dst);
			dst.RaiseEventTileSwitchAnimEnded(src);
		});
		
		src.RaiseEventTileSwitchAnimBegan(dst);
		dst.RaiseEventTileSwitchAnimBegan(src);
		
//		srcToDstAnim.Play();
//		dstToSrcAnim.Play();
	}
	
	//TODO: temporary test method for intro anim
	public void StartIntroAnim() {
//		Tweener lastTweener = null;
//		for(int i = 0; i < boardData.NumColumns; i++) {
//			boardData.ApplyActionToColumn(i, (boardPiece) => {
//				Vector3 animateToPos = boardPiece.LocalPosition;
//				if (boardPiece.Tile != null) {
//					boardPiece.Tile.cachedTransform.Translate(0f, 20f, 0);
//					boardPiece.Tile.enabled = false;
//					(boardPiece.Tile as Match3Tile).GravityEnabled = false;
//	
//					lastTweener = HOTween.To(boardPiece.Tile.cachedTransform, 1.5f,
//											 new TweenParms().Prop("localPosition", animateToPos)
//											 .Ease(EaseType.EaseOutElastic, 0f, 1.3f).Delay(i / 7.5f));
//				}
//			});
//		}
//		
//		if (lastTweener == null) {
//			Debug.LogError("No tiles to animate!");
//		} else {		
//			lastTweener.ApplyCallback(CallbackType.OnComplete, () => {
//				boardData.ApplyActionToAll((boardPiece) => {
//					if (boardPiece.Tile != null) {
//						boardPiece.Tile.enabled = true;
//						(boardPiece.Tile as Match3Tile).GravityEnabled = true;
//					}
//				});
//				
////				(lastTweener.target as Transform).GetComponent<AbstractTile>().enabled = true;
//				Debug.Log("Finished Intro Anim!");
//				
				if (OnIntroAnimFinished != null) {
					OnIntroAnimFinished(this);
				}
				
				OnBoardIntroAnimFinished.RaiseEvent();
//			});
//		}
	}

}
