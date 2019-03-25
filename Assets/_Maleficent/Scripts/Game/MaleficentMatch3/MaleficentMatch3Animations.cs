using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class MaleficentMatch3Animations : MonoBehaviour {

	public static void PerformTileUserSelectionAnimation(AbstractTile tileToAnimate)
	{
		Vector3 originalScale = tileToAnimate.transform.localScale;
		Vector3 newScale = originalScale * 0.8f;

		TweenParms parms1 = new TweenParms();
		parms1.Prop("localScale", newScale);
		parms1.Delay(0.2f);

		TweenParms parms2 = new TweenParms();
		parms2.Prop("localScale", originalScale);
		parms2.Delay(0.3f);

		Sequence sequence = new Sequence();
		sequence.Append(HOTween.To(tileToAnimate.transform, 0.2f, parms1));
		sequence.Append(HOTween.To(tileToAnimate.transform, 0.2f, parms2));

		sequence.Play();
	}
}
