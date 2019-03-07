using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class Sword : BasicItem
{
	
	public int destroyLimit = 10;
	protected int destroyIndex = 0;
	
	public float criticalMagnitudeAmount;
	
	public float minMagnitudeAmount;
	
	public float swordMovementSpeed = 8f;
	
	public float fadeOutAnimSpeed = 1f;
	
	protected float rotationAmountZ;
	
	protected Vector3 startPosition;
	protected Vector3 endPosition;
	
	protected Vector3 swipeDirecion;
	
	protected Tweener positionTweener;
	protected Tweener rotationTweener;
	
	protected Transform swordEffectInstance;
	protected SwordCollision swordCollision;
	protected Animation swordAnimation;
	protected Transform swordEffectInstanceLama;
	
	protected BoardPieceTouchController touchController;

	public override string ItemName {
		get {
			return "Sword";
		}
	}
	
	
	protected override void Awake ()
	{
		base.Awake ();
		
		touchController = GetComponent<BoardPieceTouchController>();
		touchController.OnNewBoardPieceSelected += OnNewBoardPieceSelected;
	}
	
	public override void StartUsingItem(Match3BoardGameLogic _boardLogic)
	{
		base.StartUsingItem(_boardLogic);
		
		minMagnitudeAmount = Match3BoardRenderer.horizTileDistance * 4;
		criticalMagnitudeAmount = Match3BoardRenderer.horizTileDistance;
		
		destroyIndex = 0;
		
		startPosition = Vector2.zero;
		startPosition = Vector2.zero;
	
//		TileSwitchInput.Instance.gameObject.SetActive(false);
		TileSwitchInput.Instance.DisableInput();
			
		StartCoroutine("GetInput");
	}
	
	public override void CancelUsingItem()
	{	
		StopCoroutine("GetInput");
		
		touchController.StopInputController();
		touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
		
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		
		base.CancelUsingItem();
	}
	
	protected override void DoItem()
	{	
//		Debug.LogWarning("[DoItem]");
		
		ActuallyUsingItem();
		
		SoundManager.Instance.PlayOneShot("sword_sfx");
		
		swordEffectInstance = (Instantiate(effectPrefab) as GameObject).transform;
		swordEffectInstanceLama = swordEffectInstance.GetChild(0);
		swordAnimation = swordEffectInstanceLama.GetComponentInChildren<Animation>();
		
		int rotateDirection = Vector3.Dot(Vector3.right, swipeDirecion) > 0 ? 1 : -1;
		
		swordEffectInstance.position = startPosition;
		swordEffectInstanceLama.rotation = Quaternion.LookRotation(rotateDirection * swipeDirecion.normalized);
		
		Vector3 newRot = swordEffectInstanceLama.localEulerAngles;
		newRot.y = 90f;
		swordEffectInstanceLama.localEulerAngles = newRot;
		
		swordEffectInstance.RotateAround(swordEffectInstance.position, swipeDirecion,  -rotateDirection * 25f);
		
		swordCollision = swordEffectInstance.GetComponent<SwordCollision>();
		swordCollision.OnTargetDestroyed += ActionOnTileDestroyed;
		
		if(rotateDirection == 1)
		{
			swordAnimation["effect_swordswing"].speed = 1f;
			swordAnimation.Play("effect_swordswing");
		}
		else 
		{
			swordAnimation["effect_swordswing"].speed = -1f;
			swordAnimation["effect_swordswing"].normalizedTime = 1f;
			swordAnimation.Play("effect_swordswing");
		}
		
		positionTweener = HOTween.To(swordEffectInstance.transform, swordMovementSpeed, 
											new TweenParms().Prop("position", endPosition)
											.Ease(EaseType.Linear)
											.SpeedBased()
											.OnComplete(ActionOnTweenComplete)
										 );
	}
	
	//TweenCallback
	protected void ActionOnTweenComplete()
	{
//		TileSwitchInput.Instance.gameObject.SetActive(true);
		TileSwitchInput.Instance.EnableInput();
		
		base.DoItem();
		StartCoroutine(ActionOnTweenCompleteCoroutine());
	}
	
	protected IEnumerator ActionOnTweenCompleteCoroutine()
	{
		swordAnimation["effect_sword"].speed = fadeOutAnimSpeed;
		swordAnimation.Play("effect_sword");
		yield return new WaitForSeconds(swordAnimation["effect_sword"].clip.length * 1/fadeOutAnimSpeed);
		
		if(swordEffectInstance)
		{	
			Destroy(swordEffectInstance.gameObject);
		}
	
		DoDestroy();
	}
	
	//Registers to [SwordCollision.OnTargetDestroyed]
	protected void ActionOnTileDestroyed()
	{
		destroyIndex++;
		
		if(destroyIndex == destroyLimit)
		{
			swordCollision.GetComponent<Collider>().enabled = false;
			positionTweener.Kill();
			ActionOnTweenComplete();
		}
	}
	
	//Coroutine for getting the input
	protected IEnumerator GetInput()
	{
		yield return null;

		while(true)
		{	
			if(CustomInput.touchCount != 0)
			{
				CustomInput.TouchInfo touch =  CustomInput.GetTouch(0);
				
				if(touch.phase == TouchPhase.Ended && endPosition == Vector3.zero)
				{
					startPosition = Vector3.zero;
				}
				
				if(touch.phase == TouchPhase.Ended && endPosition != Vector3.zero)
				{
					touchController.StopInputController();
					touchController.OnNewBoardPieceSelected -= OnNewBoardPieceSelected;
					
					OnInputReceived();
					yield break;
				}
			}
			
			yield return null;
		}
	}
	
	protected void OnNewBoardPieceSelected(AbstractBoardPiece boardPiece, CustomInput.TouchInfo touchInfo)
	{
//		Debug.LogWarning("[OnNewBoardPieceSelect]");
		
		if(BoardShuffleController.Instance.IsBoardReshuffling)
		{
			touchController.ClearLastSelection();
			return;
		}
		
		if(startPosition == Vector3.zero)
		{
			startPosition  = boardPiece.cachedTransform.position;
		}
		else
		{
			endPosition = boardPiece.cachedTransform.position;
		}
	}
	
	//Actions to be done once the input has been processed
	protected void OnInputReceived()
	{
//		Debug.LogWarning("[OnInputReceived]");
		
		swipeDirecion = endPosition - startPosition;
		
		if(swipeDirecion.magnitude < criticalMagnitudeAmount)
		{
			startPosition = Vector3.zero;
			endPosition = Vector3.zero;
			
			return;
		}
//		
		if(swipeDirecion.magnitude < minMagnitudeAmount)
		{	
//			Debug.LogError("Swipedirection magnitude: " + swipeDirecion.magnitude);
//			Debug.LogError("DesiredMagnitude: " + minMagnitudeRequired);		
			
			swipeDirecion = swipeDirecion.normalized * minMagnitudeAmount;
			endPosition = startPosition + swipeDirecion;
		}
		
//		
//		startPosition.x /= Screen.width;
//		startPosition.y /= Screen.height;
////		startPosition.z = 0;
//		
//		endPosition.x /= Screen.width;
//		endPosition.y /= Screen.height;
////		startPosition.z = 0;
//		
//		startPosition = Match3BoardGameLogic.Instance.gameCamera.ViewportToWorldPoint(startPosition);
//		endPosition = Match3BoardGameLogic.Instance.gameCamera.ViewportToWorldPoint(endPosition);
		
//		//TODO: Do not hardcode depth values. Find another way of taking care of this.
//		startPosition.z = 20f;
//		endPosition.z = 20f;
//		
//		GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = startPosition;
//		GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = endPosition;
		
//		swipeDirecion = endPosition - startPosition;
		DoItem();
	}
}
