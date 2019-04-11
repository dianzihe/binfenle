// (c) Copyright HutongGames, LLC 2010-2012. All rights reserved.

using UnityEngine;
using AnimationOrTween;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("NGUI")]
	[Tooltip("Sets the direction of a button tween animation.")]
	public class NGuiSetButtonTweenDirection : FsmStateAction
	{
		//[ObjectType(typeof(UIButtonTween))]
		[Tooltip("Because there can be more than one tweener, you will need to explicitly reference it below by dragging the component itself, not the gameObject")]
		public FsmObject OrButtonTweenComponent;
		
		public Direction playDirection;
		
		public override void Reset()
		{

			OrButtonTweenComponent = null;
			
			playDirection = Direction.Forward;
			
		}

		public override void OnEnter()
		{
			DoSetPlayDirection();

			Finish();
		}
		
		
		void DoSetPlayDirection()
		{
			/* 
			UIButtonTween tween = OrButtonTweenComponent.Value as UIButtonTween;

			if (tween == null)
			{
				return;
			}else{
				
				tween.playDirection = playDirection;
			}	
			*/
			
		}
	}
}
