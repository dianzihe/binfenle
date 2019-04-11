using System;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;
//[CustomEditor(typeof(NGuiEventsToPlaymakerFsmEvents))]
public class NGuiEventsToPlaymakerFsmEventsInspector : Editor
{
	
	public override void OnInspectorGUI()
    {
		 NGuiEventsToPlaymakerFsmEvents _target = (NGuiEventsToPlaymakerFsmEvents)this.target;
		
        EditorGUIUtility.LookLikeInspector();
        EditorGUI.indentLevel = 0;
		
		//DrawDefaultInspector();
		
		EditorGUILayout.Space();
		_target.debug = EditorGUILayout.Toggle("Debug",_target.debug);
		
		EditorGUILayout.Separator();
		
		PlayMakerFSM _targetFsm = _target.targetFSM;
		
		if (_target.targetFSM == null)
		{
			_targetFsm = _target.GetComponent<PlayMakerFSM>();
			
			if (_targetFsm!=null)
			{
				PlayMakerFSM _newTargetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("The Target defaults to",_targetFsm,typeof(PlayMakerFSM));	
				if (_newTargetFSM!=_targetFsm)
				{
					_target.targetFSM = _newTargetFSM;
				}
				//GUILayout.Label("The Target will be the first Fsm found on this GameObject:\n <"+_targetFsm.FsmName+">");
			}
			
		}else{
			_target.targetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("Fsm Target",_targetFsm,typeof(PlayMakerFSM));	
		}
		
		if (_targetFsm!=null)
		{
			EditorGUILayout.Separator();
		
			OnGUI_DrawNGuiEventImplementation(_targetFsm);
		}else{
			
			_target.targetFSM = (PlayMakerFSM)EditorGUILayout.ObjectField("Fsm Target",_targetFsm,typeof(PlayMakerFSM));
			 EditorGUI.indentLevel = -2;
			EditorGUILayout.HelpBox("No Fsm Found. Please select one or add one to this GameObject",MessageType.Error);
			
			/*
			GUI.color = PlayMakerPhotonEditorUtility.lightOrange;
			GUILayout.BeginHorizontal("","box",GUILayout.ExpandWidth(true));
				GUI.color = Color.white;
				GUILayout.Label("No Fsm Found. Please select one or add one to this GameObject");
			GUILayout.EndHorizontal();
			*/
			
			
			if (GUILayout.Button("Add Fsm to this GameObject"))
			{
				PlayMakerFSM _new = _target.gameObject.AddComponent<PlayMakerFSM>();
				_new.FsmName = _target.gameObject.name+" NGUI Events Receiver";
				_new.FsmDescription = "Implement the NGUI / XXX global events in this FSM to start getting feedback from NGUI";
			}
		}
		
		EditorGUIUtility.LookLikeControls();
	}
	
	public void OnGUI_DrawNGuiEventImplementation(PlayMakerFSM fsm)
	{
		NGuiEventsToPlaymakerFsmEvents _target = (NGuiEventsToPlaymakerFsmEvents)this.target;
		
		
		bool _noImplementation = true;
		
		foreach (NGuiPlayMakerDelegates _value in Enum.GetValues(typeof(NGuiPlayMakerDelegates)))
		{
			string _fsmEvent = NGuiPlayMakerProxy.GetFsmEventEnumValue(_value);
			
			int _counter = _target.getUsage(_value);
		//	if (Application.isPlaying)
		//	{
				//_fsmEvent  += " "+_target.getUsage(_value);
		//	}
			
			string _feedback = "Not implemented";
			Color _color = Color.white;
			
			if (_target.DoesTargetImplementsEvent(fsm,_fsmEvent))
			{
				_noImplementation = false;
				_feedback = "";
				_color = Color.green;
			}else{ 
				
				if (_target.DoesTargetMissEventImplementation(fsm,_fsmEvent))
				{
					//_color = PlayMakerPhotonEditorUtility.lightOrange;
					_feedback = "Not used";
				}
			
				
			}
			if (_counter>0)
			{
				_feedback += " "+_counter.ToString();		
			}
			GUI.color = _color;
			GUILayout.BeginHorizontal("","box",GUILayout.ExpandWidth(true));
				GUI.color = Color.white;
				
				EditorGUILayout.LabelField(_fsmEvent,_feedback);

			GUILayout.EndHorizontal();
			
		}
		
			
			if (_noImplementation)
			{
				EditorGUI.indentLevel = -2;
				EditorGUILayout.HelpBox("The Fsm Target does not implement any NGUI Events. Edit your Fsm to add Global Transitions or State Transitions from 'Custom Events/NGUI'",MessageType.Error);
			}
	}
}