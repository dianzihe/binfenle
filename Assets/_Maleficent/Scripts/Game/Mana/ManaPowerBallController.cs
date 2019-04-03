using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using System.Linq;

public class ManaPowerBallController : MonoBehaviour, ISelectableItem{

	//Evens for communication between portrait & landscape ManaBall
	public static event System.Action<ManaPowerBallController> OnManaPowerBallWillOpen;
	public static event System.Action<ManaPowerBallController> OnManaPowerBallWillClose;
	public event System.Action<ISelectableItem> SelectionEvent;
	public event System.Action<ISelectableItem> EndSelectionEvent;

	public static bool isOpen = false;
	private static ManaPowerBallController overlayMaterialUpdater;


	public Match3BoardGameLogic boardLogic;

	public float manaBallFillLimit = 100f;
	public float fillAnimationDuration = 0.7f;
	public float idleRotationAnimationDuration = 10f;
	public Vector3 manaBallFilledScale;
	public Transform availableManaBall;
	public UILabel addManaLabel;
	public UILabel availableManaLabel;

	public float openAnimationDuration = 0.5f;
	public Vector3 powerUpOpenScale;
	public Transform[] powerUpsTransforms;
	public ManaItemHolder powerUpInvoker;
	public ManaPowerBallController twinItem;
	public Color initialOverlayColor;
	public Color finalOverlayColor;
	public Renderer overlay;

	public AudioClip soundEffectOpen;
	public AudioClip soundEffectClose;

	private List<ManaPowerUpSelector> powerUpsSelectors;
	private List<Vector3> powerUpsOpenPositions;
	private Transform powerUpInvokerContainer;
	private WaitForEndOfFrame waitEndFrame;
	private bool allInputsEnabled = true;
	private bool updatingMana = false;
	private bool transitioning = false;
	private bool open = false;
	private bool itemSelectedInThis = false;
	private bool ignoreInputs = false;

	static string FormatAmount(int amount)
	{
		return amount >= 1000 ? amount.ToString("0,0").Replace(",", Language.Get("SCORE_SEPARATOR").Replace("<SPACE>"," ")) : amount.ToString();
	}
	
	public bool Transitioning {
		get{
			return transitioning;
		}set {
			transitioning = value;
		}
	}

	public bool EnableAllInputs {
		get {
			return allInputsEnabled;
		}set {
			if(allInputsEnabled != value) {
				allInputsEnabled = value;
				foreach(ManaPowerUpSelector pUp in powerUpsSelectors) {
					if(pUp.Item) {
						pUp.GetComponent<Collider>().enabled = allInputsEnabled;
					}else {
						pUp.GetComponent<Collider>().enabled = false;
					}

				}
			}
		}
	}

	#region Object life cycle
	
	void Awake () 
	{
		BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>() as BoxCollider;
		boxCollider.size = new Vector3(availableManaBall.localScale.x, availableManaBall.localScale.y, 1f);
		boxCollider.center = new Vector3(0, availableManaBall.localScale.y/2f, 0f);

		availableManaBall.localScale = Vector3.zero;
		availableManaLabel.text = "";
		ManaItem.OnActuallyUsingAnyItem += HandleOnActuallyUsingAnyItem;
		Match3BoardGameLogic.OnStartGame += HandleOnStartGame;
		waitEndFrame = new WaitForEndOfFrame();

		powerUpsOpenPositions = new List<Vector3>(powerUpsTransforms.Length);
		powerUpsSelectors = new List<ManaPowerUpSelector>(powerUpsTransforms.Length);

		foreach(Transform t in powerUpsTransforms) {
			Vector3 position = new Vector3(t.localPosition.x, t.localPosition.y, 0);
			powerUpsOpenPositions.Add(position);

			if(powerUpOpenScale == Vector3.zero) {
				powerUpOpenScale = t.localScale;
			}
		}
		powerUpInvoker.manaPowerBall = this;
		powerUpInvoker.OnFinishUsingItem += HandleOnFinishUsingItem;
		powerUpInvoker.OnCancelUsingItem += HandleOnCancelUsingItem;
		powerUpInvokerContainer = powerUpInvoker.transform.parent;
		ToggleInvokerVisibility(false, false);

		ManaPowerUpSelector.OnPowerUpSelected += HandleOnPowerUpSelected;
		TokensSystem.OnManaModified += HandleOnManaModified;
	}
		
	void Start () {
		BeginIdleAnimation();
		TileSwitchInput.Instance.DisableInput();
		Close (false, true);
		boardLogic.loseConditions.OnLoseChecked += HandleOnLoseChecked;
		boardLogic.winConditions.OnWinChecked += HandleOnWinChecked;
	}

	void OnDestroy () 
	{
		overlayMaterialUpdater = null;
		ManaPowerBallController.isOpen = false;

		boardLogic.loseConditions.OnLoseChecked -= HandleOnLoseChecked;
		boardLogic.winConditions.OnWinChecked -= HandleOnWinChecked;

		powerUpInvoker.OnFinishUsingItem -= HandleOnFinishUsingItem;
		powerUpInvoker.OnCancelUsingItem -= HandleOnCancelUsingItem;

		ManaPowerUpSelector.OnPowerUpSelected -= HandleOnPowerUpSelected;
		ManaItem.OnActuallyUsingAnyItem -= HandleOnActuallyUsingAnyItem;
		Match3BoardGameLogic.OnStartGame -= HandleOnStartGame;
		TokensSystem.OnManaModified -= HandleOnManaModified;
	}

	#endregion

	#region Initial Setup

	void BeginIdleAnimation() 
	{
		TweenParms parms = new TweenParms().Prop("eulerAngles", new Vector3(0, 0, -360), true).Loops(-1).Ease(EaseType.Linear);
		HOTween.To (availableManaBall, idleRotationAnimationDuration, parms);
	}

	#endregion

	#region Items

	public void SetAvailableItems(GameObject _itemsGameObject) {
		CharacterItems items = _itemsGameObject.GetComponent<CharacterItems>();
		int nItems = items.itemsList.Count;
		int nAvailableItems = 0;

		for (int i = 0; i < powerUpsTransforms.Length; i++) {
			ManaPowerUpSelector selector = powerUpsTransforms[i].GetComponentInChildren<ManaPowerUpSelector>();

			if (selector != null) {
				powerUpsSelectors.Add(selector);
				
				if (i < nItems) {
					ManaItem item = items.itemsList[i];
					
					if(item.IsAvailable()) {
						selector.Item = item;
						powerUpsSelectors.Add(selector);
						nAvailableItems++;
					}else {
						selector.Item = null;
					}
				}
			}
		}

		if (0 == nAvailableItems) {
			GetComponent<Collider>().enabled = false;
		}
	}

	#endregion
	
	#region Update Mana

	void UpdateManaFillAmount (bool animated = true)
	{
		float fillAmount = Mathf.Clamp(TokensSystem.Instance.ManaPoints/manaBallFillLimit, 0f, 1f); 
		Vector3 localScale = Vector3.Max(fillAmount * manaBallFilledScale, new Vector3(0.1f, 0.1f, 0.1f));

		if(animated) {
			TweenParms parms = new TweenParms().Prop("localScale", localScale).Ease(EaseType.EaseInCirc);
		HOTween.To (availableManaBall, fillAnimationDuration, parms);
		}else {
			availableManaBall.localScale = localScale;
		}
	}
	
	IEnumerator UpdateManaCourutine (int amount)
	{
		if(!updatingMana)
		{
			updatingMana = true;
			yield return waitEndFrame;
			
			UpdateManaFillAmount(true);

			int manaPoints = TokensSystem.Instance.ManaPoints;

			if (manaPoints > 0) {
				availableManaLabel.text = ManaPowerBallController.FormatAmount(manaPoints);
			}else {
				availableManaLabel.text = "";
			}


			if (amount > 0) {
				addManaLabel.text = "+" + amount;
				availableManaBall.GetComponent<Animation>().CrossFade("manaball_particles_add_animation");
			} else if (amount < 0) {
				addManaLabel.text = amount + "";
				availableManaBall.GetComponent<Animation>().CrossFade("manaball_particles_remove_animation");
			}

			if (amount != 0) {
				addManaLabel.GetComponent<Animation>().CrossFade("manaball_text_add_animation");
			}

			
			updatingMana = false;
		}
	}

	#endregion


	public void Open(bool animated = true, bool forced = false) 
	{
		if(!transitioning) {

			if (!open) {
				SoundManager.Instance.PlayOneShot(soundEffectOpen);
			}

			powerUpInvoker.SetItem(null);

			if(OnManaPowerBallWillOpen != null) {
				OnManaPowerBallWillOpen(this);
			}

			if(animated) {
				SoundManager.Instance.PlayQueued("manaball_open_sfx");
			}

			ToggleBallState(true, animated, forced);

			open = true;
			ManaPowerBallController.isOpen = true;
		}
	}

	public void Close(bool animated = true, bool forced = false)
	{
		if(!transitioning || forced) {

			if (open) {
				SoundManager.Instance.PlayOneShot(soundEffectClose);
			}

			if(OnManaPowerBallWillClose != null) {
				OnManaPowerBallWillClose(this);
			}

			if(animated) {
				SoundManager.Instance.PlayQueued("manaball_close_sfx");
			}

			ToggleBallState(false, animated, forced);

			open = false;
			ManaPowerBallController.isOpen = false;
		}
	}

 	void ToggleBallState (bool open, bool animated, bool forced) 
	{
		if (!transitioning || forced) {
			transitioning = animated;

			Color overlayColor = initialOverlayColor;
			if (open) {
				TileSwitchInput.Instance.DisableInput();
				overlayColor = finalOverlayColor;
			}

			if(overlayMaterialUpdater == null) {
				overlayMaterialUpdater = this;

				if (animated) {
					TweenParms parms = new TweenParms().Prop ("color", overlayColor).Ease (EaseType.Linear).Delay(0.5f);
					HOTween.To (overlay.sharedMaterial, openAnimationDuration, parms);
				}else {
					overlay.sharedMaterial.color = overlayColor;
				}
			}


			for(int i = 0; i < powerUpsTransforms.Length; i++) {
				Transform t = powerUpsTransforms[i];

				ManaPowerUpSelector pUpSelector = null;
				if(i < powerUpsSelectors.Count) {
					pUpSelector = powerUpsSelectors[i];
				}

				float alpha = 0f;
				Vector3 position = Vector3.zero;
				Vector3 localScale = Vector3.zero;

				if(open) {
					alpha = 1f;
					position = powerUpsOpenPositions[i];
					localScale = powerUpOpenScale;
				}

				if(animated) {
					TweenParms parms;
					EaseType easeType = open?EaseType.EaseOutSine:EaseType.EaseInSine;

					parms = new TweenParms().Prop("localPosition", position).Prop("localScale", localScale).Ease(easeType).Delay(0.5f);

					if(i == powerUpsTransforms.Length - 1) {
						parms.OnComplete(OpenCloseAnimationCallback);
					}

					HOTween.To(t, openAnimationDuration, parms);
					if(pUpSelector != null) {
						HOTween.To (pUpSelector.manaCostLabel, openAnimationDuration, "alpha", alpha);
					}
				}else {
					t.localPosition = position;
					t.localScale = localScale;
					if(pUpSelector != null) {
						pUpSelector.manaCostLabel.alpha = alpha;
					}

					if(i == powerUpsTransforms.Length - 1) {
						OpenCloseAnimationCallback();
					}
				}
			}
		}
	}

	void ToggleInvokerVisibility(bool show, bool animated) 
	{
		Vector3 scale = show? Vector3.one:Vector3.zero;
		float alpha = show?0f:1f;

		if(animated) {
			TweenParms parms = new TweenParms().Prop("localScale", scale);

			if(show) {
				parms.OnComplete(ShowInvokerAnimationCallback);
			}else {
				parms.OnComplete(HideInfokerAnimationCallback);
			}
			HOTween.To(powerUpInvokerContainer, openAnimationDuration, parms);
			HOTween.To (availableManaLabel, openAnimationDuration, "alpha", alpha);
		}else {
			powerUpInvokerContainer.localScale = scale;
			availableManaLabel.alpha = alpha;
		}
	}
	

	void OpenCloseAnimationCallback () 
	{
		transitioning = false;
		overlayMaterialUpdater = overlayMaterialUpdater == this?null:overlayMaterialUpdater;
		if(open) {
			overlay.GetComponent<Collider>().enabled = true;
		}else {
			overlay.GetComponent<Collider>().enabled = false;
			TileSwitchInput.Instance.EnableInput();
		}
	}

	void ShowInvokerAnimationCallback()
	{
		if(itemSelectedInThis) {
			powerUpInvoker.ActionOnClick();
		}
	}

	void HideInfokerAnimationCallback()
	{
	}

	#region NGUI events

	void OnClick() 
	{
		if(ignoreInputs) {
			return;
		}

		if(SelectionEvent != null) {
			SelectionEvent(this);
		}

		if(!ManaItemHolder.usingItem && allInputsEnabled) 
		{
			if(open) {
				twinItem.Close();
				Close();
			}else 
			{
				ManageManaBtnState();
				twinItem.Open();
				Open(true);
			}
		}
	}

	#endregion
	void ManageManaBtnState()
	{

	}
	#region Event Handlers

	void HandleOnStartGame ()
	{
		StartCoroutine(UpdateManaCourutine(0));
	}
	
	void HandleOnActuallyUsingAnyItem (BasicItem item)
	{
		StartCoroutine(UpdateManaCourutine(0));
	}
	
	void HandleOnPowerUpSelected (ManaPowerUpSelector obj)
	{
		Close();

		if (powerUpsSelectors.Contains(obj)) {
			itemSelectedInThis = true;
			powerUpInvoker.SetItem(obj.Item.gameObject);
			twinItem.powerUpInvoker.SetItem(obj.Item.gameObject);
		}else {
			itemSelectedInThis = false;
		}

		ToggleInvokerVisibility(true, true);
	}

	void HandleOnFinishUsingItem ()
	{
		ToggleInvokerVisibility(false, true);
		twinItem.ToggleInvokerVisibility(false, true);
	}

	void HandleOnCancelUsingItem ()
	{
		HandleOnFinishUsingItem();	
	}

	void HandleOnWinChecked ()
	{
		ignoreInputs = true;
		if(open) {
			Close(true, true);
		}
	}
	
	void HandleOnLoseChecked ()
	{
		if(open) {
			Close(true, true);
		}
	}

	void HandleOnManaModified (int amount)
	{
		StartCoroutine(UpdateManaCourutine(amount));
	}
	
	#endregion

	#region ISelectableItem
	
	public string SelectableItemName ()
	{
		return "ManaPowerBall";
	}
	
	public void EnableItem(bool enable)
	{
		ignoreInputs = !enable;
	}
	
	#endregion
}
