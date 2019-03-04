using UnityEngine;
using System.Collections;

public class DestroyFrostLabel : MonoBehaviour 
{
	public Match3BoardGameLogic gameLogic;
	
	public GameObject targetScore;
	
	UILabel label;
	WinDestroyTilesFrost winCondition;
	bool updatingValues = false;
	
	protected WaitForEndOfFrame waitEndFrame;
	
	
	void Awake() 
	{
		waitEndFrame = new WaitForEndOfFrame();
	}

	void Start () 
	{
		winCondition = (gameLogic.winConditions as WinDestroyTilesFrost);
		
		if (winCondition == null) {
			Destroy(transform.parent.gameObject);
			return;
		}
		
		if (targetScore != null) {
			targetScore.SetActive(false);
		}
		
		label = GetComponent<UILabel>();
		StartCoroutine(UpdateValues());
		
		LayeredBoardPiece.OnNumLayersDecreased += OnFrostChanged;
		LayeredBoardPiece.OnLayeredBoardPieceInit += OnFrostChanged;
	}
	
	void OnFrostChanged(LayeredBoardPiece piece)
	{
		if (!updatingValues) {
			StartCoroutine(UpdateValues());
		}
	}
	
	IEnumerator UpdateValues() 
	{
		updatingValues = true;
		yield return waitEndFrame;
		
		label.text = Mathf.Max(0, winCondition.frostedBoardPiecesCount).ToString();
		
		if (winCondition.frostedBoardPiecesCount <= 0 && targetScore != null) 
		{
			targetScore.SetActive(true);
			transform.parent.gameObject.SetActive(false);
		}
		
		updatingValues = false;
	}
	
	void OnDestroy()
	{
		LayeredBoardPiece.OnNumLayersDecreased -= OnFrostChanged;
		LayeredBoardPiece.OnLayeredBoardPieceInit -= OnFrostChanged;
	}
}
