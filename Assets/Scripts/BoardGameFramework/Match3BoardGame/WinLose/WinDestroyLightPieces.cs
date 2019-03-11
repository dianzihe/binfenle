using UnityEngine;
using System.Collections;

public class WinDestroyLightPieces : WinScore
{
	public static event System.Action<WinDestroyLightPieces> OnLightMatchesNeededUpdated;
	
	public int lightPiecesCount;
	public int notLightenPieces;

	#region protected
	protected override void Awake()
	{
		Debug.Log("**** AWAKE ****");
		base.Awake();

		IlluminableBoardPiece.OnMatchCounterBoardPieceInit += HandleOnMatchCounterBoardPieceInit;
		IlluminableBoardPiece.OnNewMatch += HandleOnNewMatch;
	}

	protected override void Start()
	{
        System.Console.WriteLine("****windestroyLightPieces START ****");
		base.Start();

		StartCoroutine(_PopulateEmpty());
	}


	IEnumerator _PopulateEmpty()
	{
		yield return new WaitForEndOfFrame();

		GameObject illuminableBackground = Resources.Load("Game/Tiles/IlluminableModelBack") as GameObject;

		//Match3BoardPiece bp = new Match3BoardPiece();
		Match3BoardGameLogic.Instance.boardData.ApplyActionToAll((boardPiece) => {
			if (typeof(Match3BoardPiece) == boardPiece.GetType()) {
                /*
				GameObject illuminableCopy = GameObject.Instantiate(illuminableBackground) as GameObject;
				illuminableCopy.transform.parent = boardPiece.transform;
				illuminableCopy.transform.localPosition = new Vector3(0, 0, 1);
                */
			}
		});
	}

	protected override void OnDestroy () 
	{
		base.OnDestroy();
		IlluminableBoardPiece.OnMatchCounterBoardPieceInit -= HandleOnMatchCounterBoardPieceInit;
		IlluminableBoardPiece.OnNewMatch -= HandleOnNewMatch;
	}

	protected virtual void HandleOnMatchCounterBoardPieceInit (MatchCounterBoardPiece obj)
	{
		lightPiecesCount++;
		notLightenPieces = lightPiecesCount;
	}
	
	protected void HandleOnNewMatch (MatchCounterBoardPiece obj)
	{
		if(!AllPiecesLighten()) {

			IlluminableBoardPiece illuminablePiece = obj as IlluminableBoardPiece;
			if(illuminablePiece.IsLighten()) {
				notLightenPieces--;

				if(null != OnLightMatchesNeededUpdated) {
					OnLightMatchesNeededUpdated(this);
				}
				
				CheckWin();
			}
		}
	}

	#endregion
	
	#region public

	public override float CalculateObjectiveProgress()
	{
		if (notLightenPieces != lightPiecesCount) {
			return (1f - notLightenPieces / (float)lightPiecesCount) * 0.9f;
		}
		
		return 0.9f + base.CalculateObjectiveProgress() * 0.1f;
	}

	public override bool Check()
	{
		bool allPiecesLighten = AllPiecesLighten();
		
		if (allPiecesLighten) {
			UpdateMinimumWinRequirement();
		}
		
		return base.Check() && allPiecesLighten;
	}

	public bool AllPiecesLighten () 
	{
		return notLightenPieces == 0;
	}

	public override string GetLoseReason()
	{
		if (AllPiecesLighten()) {
			return base.GetLoseReason();
		}
		else {
			return Language.Get("LIGHT_BOARD_PIECES_LOSE");
		}
	}
	
	public override string GetObjectiveString()
	{
		return Language.Get("GAME_OBJECTIVE_LIGHT_BOARD_PIECES");
	}
	
	public override string GetShortObjectiveString(AbstractLoseCondition loseConditions)
	{
		return Language.Get("MAP_OBJECTIVE_LIGHT_BOARD_PIECES");
	}
	
	public override string GetLevelType (AbstractLoseCondition loseConditions)
	{
		return "LIGHT_BOARD_PIECES";
	}
	
	#endregion

}

