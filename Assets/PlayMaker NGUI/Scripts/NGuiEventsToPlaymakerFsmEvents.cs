// (c) copyright Hutong Games, LLC 2010-2013. All rights reserved.
using System;
using HutongGames.PlayMaker;
using UnityEngine;

/// <summary>
/// Put this component on the GameObject with the Collider used by NGUI.
/// Choose an FSM to send events to (leave blank to target an FSM on the same GameObject).
/// You can rename the events to match descriptive event names in your FSM. E.g., "OK Button Pressed"
/// NOTE: Use the Get Event Info action in PlayMaker to get event arguments.
/// See also: http://www.tasharen.com/?page_id=160
/// </summary>
public class NGuiEventsToPlaymakerFsmEvents : MonoBehaviour
{
	public static bool justLoaded = true;
	
	public bool debug = true;
	
	public static UICamera.MouseOrTouch currentTouch;
	
	public PlayMakerFSM targetFSM;
	
	private int[] _usage;
	
	
	void Awake() 
	{
		debug = false;
		/* 
		if (InputDisableOnLoad.instance == null)
		{
			justLoaded = false;
		}
		*/
	}
	
	public int getUsage(NGuiPlayMakerDelegates fsmEventDelegate)
	{
		//Debug.Log("get usage for "+fsmEventDelegate);
		if (_usage==null)
		{
			return 0;
		}
		int index = (int)fsmEventDelegate;
		//Debug.Log("get usage for index"+index);
		
		if (index>=_usage.Length)
		{
			return -1;
		}
		
		return _usage[index];
	}
	
	void OnEnable()
	{
		if (_usage==null || _usage.Length==0)
		{
			_usage = new int[Enum.GetNames(typeof(NGuiPlayMakerDelegates)).Length];
		}
		
		if (targetFSM == null)
		{
			targetFSM = GetComponent<PlayMakerFSM>();
		}

		if (targetFSM == null)
		{
			enabled = false;
			Debug.LogWarning("No Fsm Target assigned");
		}

	}
	
	public bool DoesTargetMissEventImplementation(PlayMakerFSM fsm, NGuiPlayMakerDelegates fsmEventDelegate)
	{
		return DoesTargetMissEventImplementation(fsm,NGuiPlayMakerProxy.GetFsmEventEnumValue(fsmEventDelegate));
	}
	
	public bool DoesTargetMissEventImplementation(PlayMakerFSM fsm, string fsmEvent)
	{
		if (DoesTargetImplementsEvent(fsm,fsmEvent))
		{
			return false;
		}
		
		foreach(FsmEvent _event in fsm.FsmEvents)
		{
			if (_event.Name.Equals(fsmEvent))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public bool DoesTargetImplementsEvent(PlayMakerFSM fsm, NGuiPlayMakerDelegates fsmEventDelegate)
	{
		return DoesTargetImplementsEvent(fsm,NGuiPlayMakerProxy.GetFsmEventEnumValue(fsmEventDelegate));
	}
	
	public bool DoesTargetImplementsEvent(PlayMakerFSM fsm, string fsmEvent)
	{
		foreach(FsmTransition _transition in fsm.FsmGlobalTransitions)
		{
			if (_transition.EventName.Equals(fsmEvent))
			{
				return true;
			}
		}
		
		foreach(FsmState _state in fsm.FsmStates)
		{
			
			foreach(FsmTransition _transition in _state.Transitions)
			{
				
				if (_transition.EventName.Equals(fsmEvent))
				{
					return true;
				}
			}
		}

		return false;
	}
	
	void FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates _event)
	{
		targetFSM.SendEvent(NGuiPlayMakerProxy.GetFsmEventEnumValue(_event));
	}

	void OnClick()
	{
		if (justLoaded)
		{
			return;
		}
		
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnClickEvent] ++;
		
		if (debug)	Debug.Log("NGuiEventsToPlaymakerFsmEvents OnClick() "+_usage[(int)NGuiPlayMakerDelegates.OnClickEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);

		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnClickEvent);
	}
	
	
	void OnHover(bool isOver)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnHoverEvent] ++;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnHover("+isOver+") "+_usage[(int)NGuiPlayMakerDelegates.OnClickEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName,this);
		Fsm.EventData.BoolData = isOver;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnHoverEvent);
	}

	void OnPress(bool pressed)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnPressEvent] ++;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnPress("+pressed+") "+_usage[(int)NGuiPlayMakerDelegates.OnPressEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = pressed;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnPressEvent);
	}

	void OnSelect(bool selected)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSelect("+selected+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = selected;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSelectEvent);
	}

	void OnDrag(Vector2 delta)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnDrag("+delta+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.Vector3Data = new Vector3(delta.x, delta.y);
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnDragEvent);
	}
	
	void OnDrop(GameObject go)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnDrop("+go.name+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.GameObjectData = go;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnDropEvent);
	}

	void OnTooltip(bool show)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnTooltip("+show+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.BoolData = show;
		
		currentTouch = UICamera.currentTouch;
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnTooltipEvent);
	}
	
	void OnSubmitChange()
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSubmitChange to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSubmitEvent);
	}
	
	void OnSliderChange(float value)
	{
		if (!enabled || targetFSM == null) return;
		
		if (debug) UnityEngine.Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSliderChange("+value+") to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.FloatData = value;
	
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSliderChangeEvent);
	}
	
	void OnSelectionChange (string item)
	{
		if (!enabled || targetFSM == null) return;
		
		_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent] ++;
		
		if (debug) Debug.Log("NGuiEventsToPlaymakerFsmEvents OnSelectionChange("+item+") "+_usage[(int)NGuiPlayMakerDelegates.OnSelectionChangeEvent]+" to "+targetFSM.gameObject.name+"/"+targetFSM.FsmName);
		
		Fsm.EventData.StringData = item;
	
		FireNGUIPlayMakerEvent(NGuiPlayMakerDelegates.OnSelectionChangeEvent);	
	}
	
	

}
