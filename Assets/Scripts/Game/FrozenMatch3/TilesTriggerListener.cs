using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class TilesTriggerListener : MonoBehaviour {
	public System.Action<NormalTile> OnTileEntered;
	public System.Action<Match3BoardPiece> OnBoardPieceEntered;
	public System.Action<NormalTile> OnTileExit;
	public System.Action<Match3BoardPiece> OnBoardPieceExit;
	
	private NormalTile tileComponent;
	private Match3BoardPiece boardPieceComponent;
	
	/// <summary>
	/// Raises the trigger enter Unity event.
	/// </summary>
	/// <param name='other'>
	/// The collider that entered this trigger.
	/// </param>
	void OnTriggerEnter(Collider other)
	{
		if (!enabled)  //|| other.gameObject.layer != Match3Globals.Instance.layerBoardTile
		{
			return;
		}
		
		tileComponent = other.GetComponent<NormalTile>();
		
		if (OnTileEntered != null && tileComponent != null)
		{
			OnTileEntered(tileComponent);
//			return;
		}
		
		boardPieceComponent = other.GetComponent<Match3BoardPiece>();
		
		if (OnBoardPieceEntered != null && boardPieceComponent != null)
		{
			OnBoardPieceEntered(boardPieceComponent);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(!enabled)
		{
			if(!enabled)
			{
				return;
			}
			
			tileComponent = other.GetComponent<NormalTile>();
			
			if(OnTileExit != null && tileComponent != null)
			{
				OnTileExit(tileComponent);
			}
			
			boardPieceComponent = other.GetComponent<Match3BoardPiece>();
		
			if (OnBoardPieceExit != null && boardPieceComponent != null)
			{
				OnBoardPieceExit(boardPieceComponent);
			}
		}
	}
}
