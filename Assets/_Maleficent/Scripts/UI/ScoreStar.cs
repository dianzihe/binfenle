using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class ScoreStar : MonoBehaviour {

	public int starIndex;
	public float appearToScaleFactor = 2f;
	public float appearSequenceDuration = 1f;

	private UISprite sprite;
	private Transform myTransform;

	void Awake () {

		myTransform = transform;
		sprite = GetComponent<UISprite>();
		sprite.alpha = 0f;

		WinScore.OnNewStarReached += HandleOnNewStarReached;
	}

	void OnDestroy () {
		WinScore.OnNewStarReached -= HandleOnNewStarReached;
	}

	void HandleOnNewStarReached (int count)
	{
		if (count == starIndex) {

			Vector3 originalScale = myTransform.localScale;
			Vector3 toScale = new Vector3(originalScale.x * appearToScaleFactor, originalScale.y * appearToScaleFactor, originalScale.z);
			float halfAnimationDuration = appearSequenceDuration/2;

			Sequence appearSequence = new Sequence();
			appearSequence.Append(HOTween.To (myTransform, halfAnimationDuration, "localScale", toScale));
			appearSequence.Append(HOTween.To (myTransform, halfAnimationDuration, "localScale", originalScale));

			HOTween.To (sprite, halfAnimationDuration, "alpha", 1);
			appearSequence.Play();
		}
	}
}
