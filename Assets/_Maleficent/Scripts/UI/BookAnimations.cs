using UnityEngine;
using System.Collections;
using System;

public class BookAnimations : MonoBehaviour {

	static BookAnimations instance;
	public static BookAnimations Instance
	{
		get {
			return instance;
		}
	}
	
	public enum BookAnimationsState
	{
		fixedState,
		fadeOutState,
		fadeInState,
		fadeOutFireState,
		fadeInFireState
	};

	class AlphaFireElement
	{
		public MapElementAlphaListener flame;
		public AvatarAlphaListener glow;
	}
	AlphaFireElement currentFire;
	UISprite backgroundOnRef;
	UISprite backgroundOffRef;

	public GameObject bookPage;
	[HideInInspector]
	public BookAnimationsState currentState;
	public event Action<float> OnFadingChapter;
	public event Action<float> OnFadingFire;
	public event System.Action OnNewChapterShown;
	CameraMovement cameraMovement;

	public AudioClip flipPageForwards;
	public AudioClip flipPageBackwards;

	// to differenciate between fade in and fade out
	//[HideInInspector]
	//public bool animating = false;

	void Awake()
	{	
		instance = this;

		cameraMovement = Camera.main.GetComponent<CameraMovement>();

		OnFadingFire += ActionOnFadingFire;

		currentState = BookAnimationsState.fixedState;
		bookPage.SetActive(false);
	}

	public void FlipToPreviousPage()
	{
		currentState = BookAnimationsState.fadeOutState;
		cameraMovement.GoToStartPosition(DoChaptersTransitionBackwards);
	}

	public void FlipToNextPage()
	{
		currentState = BookAnimationsState.fadeOutState;
		cameraMovement.GoToStartPosition(DoChaptersTransitionForwards);
	}

	// simply executes coroutine (method sent as system.action by param)
	public void DoChaptersTransitionBackwards()
	{
		StartCoroutine(DoChaptersTransitionCoroutine(false));
	}

	public void DoChaptersTransitionForwards()
	{
		StartCoroutine(DoChaptersTransitionCoroutine(true));
	}

	public IEnumerator DoChaptersTransitionCoroutine(bool forwards)
	{
		//animating = true;

		Camera.main.GetComponent<Camera>().GetComponent<BravoInputManager>().enabled = false;
		
		Camera.main.GetComponent<Camera>().GetComponent<CameraMovement>().RefreshCameraLimits();
		
		float alphaAnimationTime = 0.25f;
		// hide action

		StartCoroutine(DoAlphaAnimation(1f,0f,alphaAnimationTime,OnFadingChapter));
		yield return new WaitForSeconds(alphaAnimationTime);
		bookPage.SetActive(true);
		if(forwards)
		{
			bookPage.GetComponent<Animation>()["Take 001"].speed = 0.9f;
			bookPage.GetComponent<Animation>()["Take 001"].time = 0;
		}
		else
		{
			bookPage.GetComponent<Animation>()["Take 001"].speed = -0.9f;
			bookPage.GetComponent<Animation>()["Take 001"].time = bookPage.GetComponent<Animation>()["Take 001"].length;
		}
		bookPage.GetComponent<Animation>().Play();
		//NGUITools.PlaySound(forwards?flipPageForwards:flipPageBackwards);
		yield return new WaitForSeconds(0.7f);

		// change of chapter in the logic part
		GetComponent<ChaptersManager>().SetCurrentChapter();

		// show action
		currentState = BookAnimationsState.fadeInState;
		StartCoroutine(DoAlphaAnimation(0f,1f,alphaAnimationTime,OnFadingChapter));
		yield return new WaitForSeconds(alphaAnimationTime);
		
		bookPage.SetActive(false);
		currentState = BookAnimationsState.fixedState;
		//animating = false;
		Camera.main.GetComponent<Camera>().GetComponent<BravoInputManager>().enabled = true;

		if(OnNewChapterShown != null)
			OnNewChapterShown();
	}

	// used for both alpha animations (fade in,fade out)
	IEnumerator DoAlphaAnimation(float startAlpha, float endAlpha, float duration, Action<float> func)
	{
		float alpha = startAlpha;
		float startTime = Time.time;
		while(Time.time <= duration + startTime)
		{
			alpha = Mathf.Lerp(startAlpha,endAlpha,(Time.time - startTime)/duration);
			
			if(func != null)
				func(alpha);

			yield return null;
		}
		alpha = endAlpha;
		if(func != null)
			func(alpha);
	}

	// xavi, to do: call directly cameraMvement function
	// calculate the button to go
	public void GoToLevelButton(LoadLevelButton buttonToGo,bool move = true,System.Action func = null)
	{
		cameraMovement.GoToLevelButton(buttonToGo,move,func);
	}

	public IEnumerator DoLevelsTransition(System.Action func)
	{
		Camera.main.GetComponent<Camera>().GetComponent<BravoInputManager>().enabled = false;
		Camera.main.GetComponent<Camera>().GetComponent<CameraLimits>().active = false;

		currentState = BookAnimationsState.fadeOutFireState;
		float fadeSpeed = 1f;

		// get background on ref
		backgroundOnRef = LoadLevelButton.lastButton.transform.Find("Background").GetComponent<UISprite>();
		// create new off background for the current button
		GameObject newBackground = GameObject.Instantiate(backgroundOnRef.gameObject,backgroundOnRef.transform.position,backgroundOnRef.transform.rotation) as GameObject;
		newBackground.name = "Background_off";
		newBackground.transform.parent = backgroundOnRef.transform.parent;
		// saves background references
		backgroundOffRef = newBackground.GetComponent<UISprite>();
		backgroundOffRef.spriteName = "map_location_01_off";
		backgroundOffRef.transform.localScale = backgroundOnRef.transform.localScale;
		backgroundOffRef.alpha = 1f;
		backgroundOnRef.transform.parent.Find("Label").GetComponent<UILabel>().alpha = 0;
		backgroundOnRef.gameObject.SetActive(false);


		// fires
		AlphaFireElement fire1 = new AlphaFireElement();
		Transform fireTransform1 = LoadLevelButton.secondTolastButton.transform.Find("level_selected_fire_02(Clone)");
		fire1.flame  = fireTransform1.GetComponent<MapElementAlphaListener>();
		Transform fireTransform1Plane = fireTransform1.Find("plane");
		fire1.glow  = fireTransform1Plane.GetComponent<AvatarAlphaListener>();
		// has its default 'plane'
		fireTransform1Plane.gameObject.SetActive(false);

		// fire 2
		AlphaFireElement fire2 = new AlphaFireElement();
		Transform fireTransform2 = LoadLevelButton.lastButton.transform.Find("level_selected_fire_02(Clone)");
		fire2.flame  = fireTransform2.GetComponent<MapElementAlphaListener>();
		fire2.glow  = fireTransform2.Find("plane").GetComponent<AvatarAlphaListener>();
		fire2.flame.SetAlpha(0f);
		fire2.glow.SetAlpha(0f);

		// cam movement
		LoadLevelButton.lastButton.transform.parent.GetComponent<PathDrawer>().InitLineAnimation();
		cameraMovement.GoToLevelButton(LoadLevelButton.secondTolastButton,true);

		// animation
		currentFire = fire1;
		StartCoroutine(DoAlphaAnimation(1f,0f,fadeSpeed,OnFadingFire));

		// stop
		yield return new WaitForSeconds(1f);
		LoadLevelButton.lastButton.transform.parent.GetComponent<PathDrawer>().DoLineAnimation();

		cameraMovement.GoToLevelButton(LoadLevelButton.lastUnlockedLevelInChapter,true);

		yield return new WaitForSeconds(1f);

		// fire 2
		currentFire = fire2;
		currentState = BookAnimationsState.fadeInFireState;
		backgroundOnRef.gameObject.SetActive(true);
		StartCoroutine(DoAlphaAnimation(0f,1f,fadeSpeed,OnFadingFire));

		// do not have to be enabled cuz it's showing level panel
		yield return new WaitForSeconds(fadeSpeed+0.1f);
		currentState = BookAnimationsState.fixedState;
		Camera.main.GetComponent<Camera>().GetComponent<CameraLimits>().active = true;
		Camera.main.GetComponent<Camera>().GetComponent<BravoInputManager>().enabled = true;

		// show level panel if needed
		if(func != null)
			func();
	}
	/*
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			Color prev = backgroundOnRef.renderer.material.color;
			backgroundOnRef.renderer.material.color = new Color(prev.r,prev.g,prev.b,0f);
		}
	}
*/
	void ActionOnFadingFire(float alpha)
	{
		if(currentState == BookAnimationsState.fadeInFireState)
		{
			backgroundOffRef.alpha = 1 - alpha;
			backgroundOnRef.alpha = alpha;
			currentFire.glow.SetAlpha(alpha);
			backgroundOnRef.transform.parent.Find("Label").GetComponent<UILabel>().alpha = alpha;
		}
		currentFire.flame.SetAlpha(alpha);
	}

	void OnDestroy()
	{
		OnFadingFire -= ActionOnFadingFire;
	}
}
