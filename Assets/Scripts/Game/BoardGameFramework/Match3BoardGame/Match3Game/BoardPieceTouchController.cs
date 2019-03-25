using UnityEngine;
using System;
using System.Collections;

public class BoardPieceTouchController : MonoBehaviour {
	
	public bool interpolate = false;
	public float lerpTreshold = 100f;
	
	public delegate void BoardPieceSelectionHandler(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo);
	
	public event System.Action OnInputStarted;
	public event BoardPieceSelectionHandler OnNewBoardPieceSelected; 
	public event System.Action OnInputEnded;
	
	protected BoardCoord prevBoardCoord;
	protected BoardCoord boardCoord;

	protected AbstractBoardPiece boardPiece;
	
	protected CustomInput.TouchInfo touchInfo;
	protected Vector3 oldTouchPosition;
	
	protected Vector3 POZ_UNASIGNED = new Vector3(-1f, -1f, -1f);

	
	protected void OnEnable()
	{
		CustomInput.mousePressed = false;
		
		prevBoardCoord.col = -1;
		prevBoardCoord.row = -1;
		
		RaiseOnInputStartedEvent();
	}
	
	protected void OnDisable()
	{
		RaiseOnInputEndedEvent();
	}
	
	public void StartInputController()
	{
		oldTouchPosition = POZ_UNASIGNED;
		enabled = true;
	}
	
	public void StopInputController()
	{
		enabled = false;
	}
	
	protected void RaiseOnInputStartedEvent()
	{
		if(OnInputStarted != null)
		{
			OnInputStarted();
		}
	}
	
	protected void RaiseOnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{
		if(OnNewBoardPieceSelected != null)
		{
			OnNewBoardPieceSelected(boardPiece, touchInfo);
		}
	}
	
	protected void RaiseOnInputEndedEvent()
	{
		if(OnInputEnded != null)
		{
			OnInputEnded();
		}
	}
	
	protected BoardCoord ConvertToBoardCoord(Vector3 position)
	{
		BoardCoord returnValue = new BoardCoord(-1, -1);
		
		position.x /= Screen.width;
		position.y /= Screen.height;
		
		Vector3 worldSpacePosition = Match3BoardGameLogic.Instance.gameCamera.ViewportToWorldPoint(position);
		Vector3 boardSpacePosition = Match3BoardRenderer.Instance.cachedTransform.InverseTransformPoint(worldSpacePosition);
		
		returnValue.col = Convert.ToInt32(boardSpacePosition.x / Match3BoardRenderer.Instance.horizontalTileDistance);
		returnValue.row = Convert.ToInt32(-boardSpacePosition.y / Match3BoardRenderer.Instance.verticalTileDistance);
		
//		Debug.LogWarning("[BoardPieceTouchController] Found: " + returnValue.col + " " + returnValue.row);
		
		return returnValue;
	}
	
	protected void CheckBoardTouch(Vector3 position, CustomInput.TouchInfo touchInfo)
	{
		boardCoord = ConvertToBoardCoord(position);
			
		if(boardCoord.row < 0 || boardCoord.row >= Match3BoardRenderer.Instance.Board.NumRows || boardCoord.col < 0 || boardCoord.col >= Match3BoardRenderer.Instance.Board.NumColumns)
		{
			return;
		}
				
		if (prevBoardCoord != boardCoord)
		{
			prevBoardCoord = boardCoord;
			CustomInput.TouchInfo modifiedTouchInfo = new CustomInput.TouchInfo(touchInfo);
			modifiedTouchInfo.position = position;

			boardPiece = Match3BoardRenderer.Instance.Board[boardCoord.row, boardCoord.col];
			RaiseOnNewBoardPieceSelected(boardPiece, modifiedTouchInfo);
		}
	}
	
	public void ClearLastSelection()
	{
		prevBoardCoord.col = -1;
		prevBoardCoord.row = -1;
		
		oldTouchPosition = POZ_UNASIGNED;
	}
	
	protected void Update ()
	{
		if(CustomInput.touchCount != 0 && !Match3BoardGameLogic.Instance.loseConditions.IsPaused)
		{
			touchInfo =  CustomInput.GetTouch(0);
			
//			Debug.LogWarning("Touchphase: " + touchInfo.phase);
			
			float deltaMagnitude = touchInfo.deltaPosition.magnitude;
			
//			Debug.LogWarning("DELTA: " + deltaMagnitude + " " + lerpTreshold);
			
			if(interpolate && oldTouchPosition != POZ_UNASIGNED && deltaMagnitude > lerpTreshold)
			{
				float lerpAmount = lerpTreshold / deltaMagnitude;
				
				for(float lerpIndex = 0f; lerpIndex < 1; lerpIndex = Mathf.Clamp01(lerpIndex + lerpAmount))
				{   
				    Vector3 lerpedPosition = Vector3.Lerp(oldTouchPosition, touchInfo.position, lerpIndex);
//					Debug.LogWarning("Lerped: " + lerpedPosition.ToString());
					CheckBoardTouch(lerpedPosition, touchInfo);
				}
			}
			else
			{
				CheckBoardTouch(touchInfo.position, touchInfo);
			}
			
			oldTouchPosition = touchInfo.position;
		}
	}
}
