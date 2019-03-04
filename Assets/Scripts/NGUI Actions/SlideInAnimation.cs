using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class SlideInAnimation : MonoBehaviour
{
	private List<Vector3> initialLocalPositions;
	protected Transform cachedTransform;

	public List<Transform> targetContainers;
	public List<Camera> containerCameras;
	public float animationTime = 1f;
	public int startPos = 2000;
	
	void Start()
	{
		startPos = Mathf.CeilToInt(startPos * (Mathf.Max(Screen.width, Screen.height) / 1024f));

		int containersCount = targetContainers.Count;
		initialLocalPositions = new List<Vector3>(containersCount);
		for (int i = 0; i < containersCount; i++) {
			Transform targetContainer = targetContainers[i];
			Camera containerCamera = containerCameras[i];

			initialLocalPositions.Add(targetContainer.localPosition);

			float y = containerCamera.WorldToViewportPoint(targetContainer.position).y;
			Vector3 targetPos = new Vector3((float)startPos / Screen.width, y, 0f);
			Vector3 position = containerCamera.ViewportToWorldPoint(targetPos);
			position.z = targetContainer.position.z;
			targetContainer.position = position;

		}
	}

	
	void SlideIn()
	{
//		HOTween.To(cachedTransform, animationTime, new TweenParms().Prop("position", initPos).Ease(EaseType.Linear));
//		HOTween.To(targetContainerLandscape, animationTime, new TweenParms().Prop("position", landscapePos).Ease(EaseType.Linear));
//		HOTween.To(targetContainerPortrait, animationTime, new TweenParms().Prop("position", portraitPos).Ease(EaseType.Linear));

		int containersCount = targetContainers.Count;
		for (int i = 0; i < containersCount; i++) {
			Transform targetContainer = targetContainers[i];
			Vector3 targetPosition = initialLocalPositions[i];

			targetContainer.localPosition = targetPosition;
		}
	}
}

