
#if ( !(UNITY_IPHONE || UNITY_ANDROID) || UNITY_EDITOR)
	#define MOUSE	
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputHandler : MonoBehaviour {
	/// <summary>
	/// Max number of simultaneous touches on the screen that the InputHandler is goinf to manage
	/// </summary>
	public const int MAX_TOUCHES = 2; 
	
	/// <summary>
	/// Pool of DeviceInputs. DeviceInputs are not created or destroyed, they are stored in this array. The
	/// DeviceInput with fingerId == 0 is stores in the position 0 and so on
	/// </summary>
	private DeviceInput[] touches = new DeviceInput[MAX_TOUCHES];
	
	/// <summary>
	/// A list of DeviceInput sorted by the time they were pressed. It acts like a stack of touches
	/// </summary>
	private List< DeviceInput > sortedTouches = new List< DeviceInput >(MAX_TOUCHES);
	
	public bool Enable { get; set; }
	
#region singleton
	
	private static InputHandler instance;
	
	private InputHandler()
	{
		for(int i = 0; i < MAX_TOUCHES; ++i)
		{
			touches[i] = new DeviceInput();
		}
		
		Enable = true;
	}
	
	public static InputHandler Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameObject("InputHandler").AddComponent< InputHandler >();
			}
			return instance;
		}
	}
	
	public static bool Instantiated() {
		return instance != null;
	}
#endregion
	
#region PublicMethods
	
	/// <summary>
	/// Number of touches currently pressed on screen
	/// </summary>
	public static int TouchCount
	{
		get 	{ return Instance.sortedTouches.Count; }
	}
	
	/// <summary>
	/// Returns the arry of touches sorted by the time they were pressed
	/// </summary>
	public static List< DeviceInput > Touches
	{
		get	{ return Instance.sortedTouches; }
	}

	public static DeviceInput[] TouchesById
	{
		get	{ return Instance.touches; }
	}
	
#endregion
	
	// Update is called once per frame
	void Update () {	
		//Update touches
		for(int i = 0; i < MAX_TOUCHES; ++i)
		{
			UpdateTouch(i);
			if(touches[i].phase == DeviceInput.Phase.Inactive)
			{	
				sortedTouches.Remove(touches[i]);
			}
			if(touches[i].phase == DeviceInput.Phase.Began)
			{	
				sortedTouches.Add(touches[i]);
			}
		}
	}
	
	
	/// <summary>
	/// Updates the DeviceInput info that contains the id passed as param
	/// </summary>
	/// <param name="_id">
	/// The id associated to the touch we are looking for <see cref="System.Int32"/>
	/// </param>
	private void UpdateTouch(int _id)
	{
		bool found = false;
		foreach(Touch unityTouch in Input.touches)
		{
			if(unityTouch.fingerId == _id && Enable) //There is a unity touch with this id, which means the fonger is pressed
			{			
				found = true;
				DeviceInput touch = touches[_id];
				
				touch.fingerId = _id;
				touch.deltaPosition = unityTouch.deltaPosition;
				touch.position = unityTouch.position;
				
				//Forget about trusting unity phases, we'll update them manually
				switch(touch.phase)
				{
					case DeviceInput.Phase.Inactive: //The touch was inactive and it is pressed now
						touch.phase = DeviceInput.Phase.Began;
						break;
					
					case DeviceInput.Phase.Began:
					case DeviceInput.Phase.Moved:
					case DeviceInput.Phase.Stationary:
						touch.phase = (unityTouch.deltaPosition == Vector2.zero) ? DeviceInput.Phase.Stationary : DeviceInput.Phase.Moved;
						break;
				}
			}
		}
#if MOUSE
		if(!found && _id == 0)
		{
			found = UpdateMouse(_id);
		}
#endif

		
		//Now here is another hack. Sometimes Unity removes the touch without setting its phase to Ended first
		//and that causes a lot of pain. In addition wa have a new phase called Inactive. So let's make a few 
		//updates on the touch in case it is not stored in the array of unity pressed touches
		if(!found)
		{
			DeviceInput touch = touches[_id];
			if(touch.phase == DeviceInput.Phase.Ended || touch.phase == DeviceInput.Phase.Canceled)
			{
				touch.phase = DeviceInput.Phase.Inactive;
			}
			else if(touch.phase != DeviceInput.Phase.Inactive)
			{
				touch.phase = DeviceInput.Phase.Ended;
			}
		}
	}
	
#if MOUSE
	/// <summary>
	/// Gets the data to update DeviceInput from the mouse
	/// </summary>
	/// <param name="_id">
	/// Id to be updated with the mouse info <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// True if the moused is currently being dragged on the screen <see cref="System.Boolean"/>
	/// </returns>
	private bool UpdateMouse(int _id)
	{
		bool found = false;
		if (Input.GetMouseButton(0) && Enable)
		{
			found = true;
			if (Input.GetMouseButtonDown(0))
			{
		    	touches[_id].phase = DeviceInput.Phase.Began;
			}
			else if (Input.GetMouseButtonUp(0))
			{
		    	touches[_id].phase = DeviceInput.Phase.Ended;
			}
			else 
			{
				touches[_id].deltaPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - touches[_id].position;
				if (touches[_id].deltaPosition == Vector2.zero)
					touches[_id].phase = DeviceInput.Phase.Stationary;
				else
					touches[_id].phase = DeviceInput.Phase.Moved;
			}
			touches[_id].position = Input.mousePosition;
			touches[_id].fingerId = _id;
		}
		return found;
	}
#endif
	
#region UTILS
	/// <summary>
	/// Gets the amount of zoom increment since the last frame
	/// </summary>
	/// <returns>
	/// the amount of zoom increment since the last frame <see cref="System.Single"/>
	/// </returns>
	public static float GetZoom()
	{
		if(Instance.sortedTouches.Count > 1 && Instance.sortedTouches[1].phase != DeviceInput.Phase.Began)
		{
			DeviceInput i0 = Instance.sortedTouches[0];
			DeviceInput i1 = Instance.sortedTouches[1];
				
			float oldDistance     = ((i0.position - i0.deltaPosition) - (i1.position - i1.deltaPosition)).magnitude;
			float currentDistance = (i0.position - i1.position).magnitude;
			return currentDistance - oldDistance;
		}
#if MOUSE
		else
		{
			return Input.GetAxis("Mouse ScrollWheel") * 30.0f;
		}
#else
		return 0.0f;
#endif
	}
	
	
	/// <summary>
	/// Gets the amount of rotation increment since the last frame
	/// </summary>
	/// <returns>
	/// the amount of rotation increment since the last frame <see cref="System.Single"/>
	/// </returns>
	public float GetRotation()
	{	
		if(sortedTouches.Count > 1)
		{			
			DeviceInput i0 = sortedTouches[0];
			DeviceInput i1 = sortedTouches[1];
			
			Vector2 oldVector     = ((i0.position - i0.deltaPosition) - (i1.position - i1.deltaPosition));
			Vector2 currentVector = (i0.position - i1.position);
			if(oldVector != currentVector)
			{	
				return Mathf.Atan2(currentVector.y, currentVector.x) - Mathf.Atan2(oldVector.y, oldVector.x);
			}
		}
		
		return 0.0f;
	}
}
#endregion
