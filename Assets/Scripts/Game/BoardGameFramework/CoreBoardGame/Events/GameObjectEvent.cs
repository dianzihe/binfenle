using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// GameObjectEvent
/// 
/// Encapsulates the info required for an event to be launched to multiple GameObjects using SendMessage.
/// Once configured, the event can be raised by calling <see cref="RaiseEvent"/> method on the instance of this class.
/// </summary>
[System.Serializable]
public class GameObjectEvent 
{
	public delegate void EventDelegate(string eventName);
	
	/// <summary>
	/// Occurs when this GameObject event is raised. Other scripts can register to this event from code also 
	/// and they will also receive the sender of the event as a parameter and the event name.
	/// </summary>
	public event EventDelegate OnEventRaised;
	
	[SerializeField]
	protected string eventName;

	[SerializeField]
	protected List<GameObject> targets = new List<GameObject>();
	
	[SerializeField]
	protected List<string> messages = new List<string>();	
	
	public GameObjectEvent() { }
	
	public GameObjectEvent(GameObject _target, string _message, string _eventName) 
	{
		Targets.Add(_target);
		Messages.Add(_message);
		EventName = _eventName;
	}
	
	public GameObjectEvent(string _eventName, GameObject[] _targets, string[] _messages) 
	{
		Targets.AddRange(_targets);
		Messages.AddRange(_messages);
		EventName = _eventName;
	}
	
	public List<GameObject> Targets 
	{
		get 
		{
			return targets;
		}
		set 
		{
			targets = value;
		}	
	}

	public List<string> Messages 
	{
		get 
		{
			return messages;
		}
		set 
		{
			messages = value;
		}
	}

	public string EventName 
	{
		get 
		{
			return eventName;
		}
		set 
		{
			eventName = value;
		}
	}
	
	
	public static void SendEventMessages(GameObject[] _targets, string[] _messages, string _eventName) 
	{
		if ( (_targets == null || _targets.Length == 0) || (_messages == null || _messages.Length == 0) ) 
		{
			return;
		}
		if (_targets.Length != _messages.Length) 
		{
			Debug.LogError("[GameObjectEvent] Error while calling static SendEvent because _targets.Length != _messages.Length: " + 
				_targets.Length + " != " + _messages.Length);
		}
		
		for(int i = 0; i < _targets.Length; i++) 
		{		
			_targets[i].SendMessage(_messages[i], _eventName, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void RaiseEvent() 
	{
		// Raise this event to any other registered scripts.
		if (OnEventRaised != null) 
		{
//			Debug.Log("Raised script event: " + EventName);
			OnEventRaised(EventName);
		}
		
		if ( (Targets == null || Targets.Count == 0) || (Messages == null || Messages.Count == 0) ) 
		{
			return;
		}

		for(int i = 0; i < Targets.Count; i++) 
		{
			for(int j = 0; j < Messages.Count; j++) 
			{
				if (Targets[i] != null)
				{
					Targets[i].SendMessage(Messages[j], EventName, SendMessageOptions.DontRequireReceiver);
//					Debug.Log("Raised event: " + Messages[j] + " -> " + EventName + " on " + Targets[i].name);
				}
			}
		}
	}	
}

#region Extension methods for GameObjects
public static class GameObjectExtensions {
	/// <summary>
	/// Registers a GameObject to a <see cref="GameObjectEvent"/>.
	/// The GameObject is added to the GameObjectEvent Targets list. If the "allowDuplicates" is set to false
	/// then a check will be done to see if the same GameObject is not already registered to this event.
	/// </summary>
	/// <param name='gameObject'>
	/// Game object.
	/// </param>
	/// <param name='gameObjEvent'>
	/// Game object event.
	/// </param>
	/// <param name='gameObjMessage'>
	/// Game object message.
	/// </param>
	/// <param name='allowDuplicates'>
	/// Allow duplicates. Default is true.
	/// If set to false there is an overhead involved in checking the the Targets objects list and the Messages list for the duplicates.
	/// </param>
	public static void RegisterToEvent(this GameObject gameObject, GameObjectEvent gameObjEvent, string gameObjMessage, bool allowDuplicates = true) 
	{
		if (gameObjEvent != null) 
		{
			if ( !allowDuplicates && gameObjEvent.Targets.Contains(gameObject) && gameObjEvent.Messages.Contains(gameObjMessage) ) 
			{
				return;
			}
			gameObjEvent.Targets.Add(gameObject);
			gameObjEvent.Messages.Add(gameObjMessage);
		}
	}
}
#endregion



	