using UnityEngine;
using System.Collections;

public class TutorialsSystem : MonoBehaviour
{
	public static event System.Action OnTutorialShow;
	public static event System.Action OnTutorialHide;
	
	protected static TutorialsSystem instance;
	public static float waitTime = 10f;
	
	public GameObject[] highlightObjects;
	public GameObject highlightEffect;
	public GameObject highlightEffectExtra;
	public string textKey;
	public Vector3 messagePosition = new Vector3(0f, -200f, 0f);
	
	public int itemIdx = -1;
	
	public Match3BoardPiece pieceToMove;
	public Match3BoardPiece pieceToMoveDestination;
	public float moveIndicatorAngle = 0f;
	
	[System.NonSerialized]
	public float animationTime = 0.3f;
	[System.NonSerialized]
	public bool disableTutorial = false;
	[System.NonSerialized]
	public bool gaveFreeItems = false;
	[System.NonSerialized]
	public bool justTap = false;

	protected GameObject[] effectsObjs;
	protected GameObject moveArrow;
	
	public static TutorialsSystem Instance {
		get {
			return instance;
		}
	}
	
	void Awake()
	{
		instance = this;
		
		if (itemIdx >= 0 && (highlightObjects == null || highlightObjects.Length < 2)) {
			highlightObjects = new GameObject[2];
		}
	}
	
	// Use this for initialization
	void Start()
	{
		Match3BoardGameLogic.OnStartGame += ShowTutorial;
		PossibleMatchesController.Instance.EnabledCount--;
	}
	
	public void GiveFreeItems(/* ItemHolder item*/)
	{
		/* 
		if (PlayerPrefs.GetInt(textKey, 0) == 0) 
		{
			PlayerPrefs.SetInt(textKey, 1);
			
			if (MaleficentBlackboard.Instance.level >= LoadLevelButton.lastUnlockedLevel) 
			{
				if (Language.Get(textKey).Contains("<TOKEN>")) {
					item.AddItems(TweaksSystem.Instance.intValues["TutorialTokens"]);
				}
				else if (textKey == "TUTORIAL_ICE_PICK_ITEM") {
					item.AddItems(TweaksSystem.Instance.intValues["TutorialIcePicks"]);
				}
				else if (textKey == "TUTORIAL_SNOWBALL_ITEM") {
					item.AddItems(TweaksSystem.Instance.intValues["TutorialSnowballs"]);
				}
				else if (textKey == "TUTORIAL_HOURGLASS_ITEM") {
					item.AddItems(TweaksSystem.Instance.intValues["TutorialHourglasses"]);
				}
				
				gaveFreeItems = true;
			}
		}
		*/
	}
	
	public virtual void ShowTutorial()
	{
		Match3BoardGameLogic.OnStartGame -= ShowTutorial;
		
		if (itemIdx >= 0) {
			if (gaveFreeItems) {
				TileSwitchInput.Instance.DisableInput();
			}
			else {
				justTap = true;
			}
		}
		else if (pieceToMove != null && pieceToMoveDestination != null) {
			TileSwitchInput.Instance.InputFilter = InputFilter;
			moveArrow = GameObject.Instantiate(Resources.Load("Game/Prefabs/Movetile_Indicator") as GameObject) as GameObject;
			moveArrow.transform.parent = pieceToMove.transform;
			Vector3 rot = moveArrow.transform.localEulerAngles;
			rot.z = moveIndicatorAngle;
			moveArrow.transform.localEulerAngles = rot;
			Vector3 pos = moveArrow.transform.localPosition;
			pos.x = 0f;
			pos.y = 0f;
			moveArrow.transform.localPosition = pos;
		}
		else {
			justTap = true;
		}
		
		if (highlightObjects != null && highlightObjects.Length > 0 && highlightEffect != null) 
		{
			effectsObjs = new GameObject[highlightObjects.Length];
		
			for (int i = 0; i < highlightObjects.Length; ++i) 
			{
				GameObject effect;
				if (i > 1 && itemIdx >= 0) {
					effect = GameObject.Instantiate(highlightEffectExtra) as GameObject;
				}
				else {
					effect = GameObject.Instantiate(highlightEffect) as GameObject;
				}
				Vector3 newScale = effect.transform.localScale;
				effect.transform.parent = highlightObjects[i].transform;
				effect.transform.localPosition = Vector3.zero;
				
				if (effect.layer == LayerMask.NameToLayer("GameGUI")) {
					effect.transform.localScale = newScale;
				}
				
				effectsObjs[i] = effect;
			}
		}
		
		if (OnTutorialShow != null) {
			OnTutorialShow();
		}
		
		StartCoroutine(WaitForTapOrTime());
	}
	
	bool InputFilter(AbstractTile selectedTile, AbstractTile destinationTile, TileMoveDirection moveDirection) 
	{
		if (pieceToMove == null || pieceToMoveDestination == null) {
			return true;
		}
		
		AbstractTile tileToMove = pieceToMove.Tile;
		AbstractTile tileToMoveDest = pieceToMoveDestination.Tile;
		
		if (destinationTile == null) 
		{
			Match3BoardGameLogic boardLogic = Match3BoardGameLogic.Instance;
			BoardCoord targetBoardPos = selectedTile.BoardPiece.BoardPosition;
			targetBoardPos.OffsetByAndClamp(boardLogic.tilesMoveDirections[(int)moveDirection], boardLogic.boardData.NumRows - 1, boardLogic.boardData.NumColumns - 1);
			destinationTile = boardLogic.boardData[targetBoardPos].Tile;
		}
		
		if (tileToMove == selectedTile && tileToMoveDest == destinationTile ||
			tileToMove == destinationTile && tileToMoveDest == selectedTile)
		{
			disableTutorial = true;
			TileSwitchInput.Instance.InputFilter = null;
			return true;
		}

		return false;
	}
	
	IEnumerator WaitForTapOrTime()
	{
		while (!disableTutorial) 
		{
			if (justTap && CustomInput.touchCount > 0) {
				disableTutorial = true;
			}
			else {
				yield return null;
			}
		}
		
		if (itemIdx >= 0) {
			TileSwitchInput.Instance.EnableInput();
		}
		
		HideHighlights();
		
		if (OnTutorialHide != null) {
			OnTutorialHide();
		}
		
//		float timeToWait = waitTime;
//		bool tapped = false;
//		
//		while (timeToWait > 0f) 
//		{
//			timeToWait -= Time.deltaTime;
//			
//			if (CustomInput.touchCount > 0) {
//				HideHighlights();
//				tapped = true;
//				timeToWait = 0f;
//			}
//			
//			yield return null;
//		}
//		
//		yield return new WaitForSeconds(animationTime);
//		
//		if (itemIdx >= 0) {
//			TileSwitchInput.Instance.EnableInput();
//		}
//		
//		if (tapped) {
//			yield break;
//		}
//		
//		while (CustomInput.touchCount  == 0) 
//		{
//			yield return null;
//		}
//		
//		HideHighlights();
	}
		
	public virtual void HideHighlights()
	{
		if (effectsObjs != null && effectsObjs.Length > 0) {
			for (int i = 0; i < effectsObjs.Length; ++i) 
			{
				Destroy(effectsObjs[i]);
			}
		}
		
		if (moveArrow) {
			Destroy(moveArrow);
		}
		
		PossibleMatchesController.Instance.EnabledCount++;
		if (justTap) {
			PossibleMatchesController.Instance.OnStableBoardEvent();
		}
	}
	
	void OnDestroy()
	{
		Match3BoardGameLogic.OnStartGame -= ShowTutorial;
	}
}

