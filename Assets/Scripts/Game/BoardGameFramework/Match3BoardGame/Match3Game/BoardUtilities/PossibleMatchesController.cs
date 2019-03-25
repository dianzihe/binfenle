using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PossibleMatchesController : MonoBehaviour 
{
	public GameObject hintEffectPrefab;
	
	// Time until a new hint will displayed on the board.
	public float showNextHintDelay = 4f;
	public PossibleMatchesFinder possibleMatchesFinder;
	
	protected List<GameObject> hintEffects = new List<GameObject>(4);
	
	private bool nextHintDisplayPending = false;
	private bool matchHintVisible = false;
	
	protected WaitForSeconds waitPollTime;
	private int enabledCount = 1;
	
	private static PossibleMatchesController instance = null;

	
	void Awake()
	{
		instance = this;
		
		waitPollTime = new WaitForSeconds(0.5f);
	}
	
	void Start() {
		Debug.Log("[PossibleMatchesController] Registered to OnStableBoardCheckingStatusChanged...");
		Match3BoardGameLogic.OnStableBoardCheckingStatusChanged += OnStableBoardCheckingStatusChangedEvent;
		Match3BoardGameLogic.OnStableBoard += OnStableBoardEvent;

		possibleMatchesFinder = new PossibleMatchesFinder(Match3BoardGameLogic.Instance.boardData);
		if (possibleMatchesFinder.Board == null) {
			Debug.LogError("[PossibleMatchesController] possibleMatchesFinder.Board was not initialized succesfully! It's NULL!");
		}		
	}
	
	public int EnabledCount
	{
		get {
			return enabledCount;
		}
		set {
			enabledCount = value;
//			Debug.LogWarning("enabled Count: " + enabledCount);
			
			if(enabledCount > 0 ) {
				enabledCount = 1;
				enabled = true;
			}
			else
			{
				enabled = false;
			}
		}
	}
	
	void OnEnable()
	{
		Debug.LogWarning("[PossibleMatchesController] Controller enabled...");
		
		if (possibleMatchesFinder != null && !Match3BoardGameLogic.Instance.IsCheckingStableBoard &&
			!nextHintDisplayPending && (BoardShuffleController.Instance == null || !BoardShuffleController.Instance.IsBoardReshuffling))
		{
			ShowLastFoundHint();
		}
	}
	
	void OnDisable() 
	{
		HideLastFoundHint();
	}
	
	public static PossibleMatchesController Instance
	{
		get {
			return instance;
		}
	}
	
	/// <summary>
	/// Event raised by the <see cref="Match3BoardGameLogic"/> when board is stable.
	/// </summary>
	public void OnStableBoardEvent()
	{
//		Debug.Log("[PossibleMatchesController] OnStableBoard event received...");
		if ( !nextHintDisplayPending ) {
			StartCoroutine("ShowNextHintAfterDelay", showNextHintDelay);
		}
	}
	
	/// <summary>
	/// Event raised by the <see cref="Match3BoardGameLogic"/> when the stable board checking status changed event.
	/// </summary>
	/// <param name='isBoardStableChecking'>
	/// Is board stable checking.
	/// </param>
	public void OnStableBoardCheckingStatusChangedEvent(bool isBoardStableChecking) 
	{
		// If the board game logic started checking if it's stable it means some board event started it and we have to stop next hint display 
		// and hide any currently displaying hints.
		if (nextHintDisplayPending && isBoardStableChecking)
		{
			nextHintDisplayPending = false;
			StopCoroutine("ShowNextHintAfterDelay");
			HideLastFoundHint();
		}
		else if ( isBoardStableChecking ) {
			HideLastFoundHint();
		}
	}

	IEnumerator ShowNextHintAfterDelay(float delay)
	{
//		Debug.Log("[PossibleMatchesController] Waiting for the next hint to display after: " + delay + " seconds");
		nextHintDisplayPending = true;
		
		yield return new WaitForSeconds(delay);
		
		// If the component is disabled or the game is over (it can be over temporarilly until the user purchases some more moves/time)
		// wait before displaying the hint.
		while(!enabled || Match3BoardGameLogic.Instance.IsGameOver || BoardShuffleController.Instance.IsBoardReshuffling) 
		{
//			Debug.LogWarning("Waiting to display...");
			yield return waitPollTime;
		}

		if (possibleMatchesFinder.FindFirstPossibleMatch())
		{
//			Debug.Log("[PossibleMatchesController] Showing last found possible match...");
			ShowLastFoundHint();
		}
//		else {
//			Debug.LogWarning("[PossibleMatchesController] No possible matches found!");
//		}

		nextHintDisplayPending = false;
	}
	
	public bool MatchHintVisible 
	{
		get {
			return matchHintVisible;
		}

		protected set {
			matchHintVisible = value;
		}
	}

	public void ShowLastFoundHint()
	{
		if (MatchHintVisible) {
			return;
		}
		
		// Check if we previously found a valid result.
		if (possibleMatchesFinder.numFoundPossibleMatches > 0)
		{	
			MatchHintVisible = true;
			
			// Check if the last found tiles are still available before displaying the hints for them.
			bool isHintValid = true;
			for(int i = 0; i < possibleMatchesFinder.numFoundPossibleMatches; i++) 
			{
				if (possibleMatchesFinder.foundPossibleMatch[i] == null) 
				{
					isHintValid = false;
					break;
				}
			}
			
			if ( !isHintValid ) 
			{
//				Debug.LogWarning("[PossibleMatchesController] Last found hint not available anymore! Triggering boar stable checker for next hint display...");
				Match3BoardGameLogic.Instance.TryCheckStableBoard();
				
				return;
			}
			
			// Display the effect for each of the posssible match tiles
			for(int i = 0; i < possibleMatchesFinder.numFoundPossibleMatches; i++)
			{
				if (possibleMatchesFinder.foundPossibleMatch[i] != null) 
				{
//					Vector3 prefabLocalScale = hintEffectPrefab.transform.localScale;
					GameObject hintEffect = Instantiate(hintEffectPrefab) as GameObject;
					
					Transform hintEffectTransform = hintEffect.transform;
					hintEffectTransform.parent = possibleMatchesFinder.foundPossibleMatch[i].BoardPiece.cachedTransform;
					hintEffectTransform.localPosition = new Vector3(0f, 0f, -1f); //Vector3.zero;
					hintEffectTransform.localScale = Vector3.one;//prefabLocalScale;
					hintEffects.Add(hintEffect);
				}
			}
		}
	}
	
	public void HideLastFoundHint()
	{
		if ( !MatchHintVisible ) {
			return;
		}

		MatchHintVisible = false;
		
		// Hide the effect for each of the possible match tiles
		for(int i = 0; i < hintEffects.Count; i++) {
			if (hintEffects[i] != null) {
				Destroy(hintEffects[i]);
			}
		}

		hintEffects.Clear();
	}

	protected void OnDestroy() 
	{
		hintEffects.Clear();
		
		Debug.Log("[PossibleMatchesController] Unregistered from OnStableBoardCheckingStatusChanged...");
		Match3BoardGameLogic.OnStableBoardCheckingStatusChanged -= OnStableBoardCheckingStatusChangedEvent;
		Match3BoardGameLogic.OnStableBoard -= OnStableBoardEvent;
	}
}
