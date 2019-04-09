using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowButton : UIControl {

	[System.Serializable]
	public enum ArrowType
	{
		Left,
		Right
	};
	
	public ArrowType direction;
	public GameObject arrowGO;
	float lastAlpha = -1;

	BookAnimations bookAnimations;

	void Awake()
	{
		BookAnimations.Instance.OnFadingChapter += SetAlpha;
	}

	void Start()
	{
		SetAlpha(1f);
	}
	
	public override void OnTouchDown(BravoInputManager.TouchCollision _collisionInfo) {
		if(BookAnimations.Instance.currentState == BookAnimations.BookAnimationsState.fixedState)
		{
			if(direction == ArrowType.Left)
				ChaptersManager.Instance.GoToPreviousChapter();
			else
				ChaptersManager.Instance.GoToNextChapter();
		}
	}

	public void SetAlpha(float alpha)
	{
		Chapter currentChapter = ChaptersManager.Instance.GetCurrentChapter();

		// cases in which the button alpha must be 0
		if(BookAnimations.Instance.currentState == BookAnimations.BookAnimationsState.fadeOutState && lastAlpha == 0)
				alpha = 0;

		if(BookAnimations.Instance.currentState == BookAnimations.BookAnimationsState.fadeInState || lastAlpha == -1)
		{
			if((direction == ArrowType.Right && currentChapter.nextChapter == null) ||
			   direction == ArrowType.Left && currentChapter.previousChapter == null)
			{
				alpha = 0f;
			}
		}

		arrowGO.GetComponent<MeshRenderer>().material.color = new Vector4(1f,1f,1f,alpha);
			
		lastAlpha = alpha;
	}

	void OnDestroy()
	{
		BookAnimations.Instance.OnFadingChapter -= SetAlpha;
	}
}
