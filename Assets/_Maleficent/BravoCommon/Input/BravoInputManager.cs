using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BravoInputManager : MonoBehaviour
{
	public bool multitouch = false; //Enable this on virtual joystick on HUDS, for example... disable it on menus

	public class TouchInfo {
		public TouchInfo(int _id) {
			id = _id;
		}
	
		public int id;
		public UIControl uicontrol;			// UIControl asociated with this info
		public BravoInputManager inputManager;	// The input manager that is controlling this UIControl
		public Vector3 collPoint; 			// The coll point on the plane asociated with the uicontrol
	}
	
	public class TouchCollision {
		public TouchCollision(TouchInfo _info, Ray _ray) {
			touchInfo = _info;
			touchRay = _ray;
		}
		
		public TouchInfo 	touchInfo;
		public Ray			touchRay;
	}
	
	public static TouchInfo[] touchInfo; // List of Info associated with each finger id. TouchInfo in position 0 is associated with the touch with id == 0 and so on
	
	private Camera inputCamera; // Camera from wich rays are thrown
	
#region BravoInputManager managers
	private static List< BravoInputManager > sortedInputManagers;
	
	private void RegisterInputManager() { 
		if(sortedInputManagers.Count == 0) {
			sortedInputManagers.Add(this);
		} else {
			int i = 0;
			while( (i < sortedInputManagers.Count) && sortedInputManagers[i].GetComponent<Camera>().depth > this.GetComponent<Camera>().depth) {
				i ++;
			}
			sortedInputManagers.Insert(i, this);
		}
	}
	
	private void DeregisterInputManager() { 
		sortedInputManagers.Remove(this);
	}
	
	static BravoInputManager() {
		touchInfo = new TouchInfo[InputHandler.MAX_TOUCHES];
		for(int i = 0; i < InputHandler.MAX_TOUCHES; ++i) {
			touchInfo[i] = new BravoInputManager.TouchInfo(i);
		}
		
		sortedInputManagers = new List<BravoInputManager>();
	}
	
#endregion
	
	public void Start() {
		inputCamera = GetComponent<Camera>();
		if(inputCamera == null)
			Debug.LogError("BravoInputManager Component only works on Cameras");
		
		RegisterInputManager();
	}
	
	void OnDestroy() {
		DeregisterInputManager();
	}
	
	void OnDisable() {
		if(InputHandler.Instantiated()){ //When stopping the application the input handler may already heve been destroyed, we must avoid creating it here again
			foreach(DeviceInput currentTouch in InputHandler.Touches) {
				TouchInfo info = touchInfo[currentTouch.fingerId];
				if(info.inputManager == this) {
					//Properly free this touch
					info.uicontrol.OnTouchCancelled();
					
					info.uicontrol.touchesIds.Remove(currentTouch.fingerId);
					
					info.uicontrol = null;
					info.inputManager = null;
				}
			}
		}
	}
	
	public void Update() {
		//Find the first input manager active
		BravoInputManager inputBoss = null;

		if(sortedInputManagers.Count > 0)
			inputBoss = sortedInputManagers[0];
	
		for(int i = 0; i < sortedInputManagers.Count; ++i) {
			if(sortedInputManagers[i].gameObject.activeInHierarchy) {
				inputBoss = sortedInputManagers[i];
				break;
			}
		}
		
		
		if(this == inputBoss) {
			//Copy the list because it is very likely that there will be changes when processing the input managers
			BravoInputManager[] cachedInputManagers = sortedInputManagers.ToArray();
		
			foreach(BravoInputManager inputManager in cachedInputManagers) {
				if(inputManager.enabled &&  inputManager.gameObject.activeInHierarchy) {
					inputManager.UpdateInputManager();
				}
			}
		}
	}

	TouchInfo infoTmp = new TouchInfo(0);
	public void UpdateInputManager () {		
		//Checking for all finger id
		foreach(DeviceInput currentTouch in InputHandler.Touches) {	
			TouchInfo info = touchInfo[currentTouch.fingerId];
			if(info.inputManager != null && info.inputManager != this) {
				continue; //Other camera is taking care of this input
			}
			
			RaycastHit[] hits;
			
			switch(currentTouch.phase) {
				case DeviceInput.Phase.Began: {		
					if(info.uicontrol == null) {
						//Check if there has been a collision with any gameobject
						Ray ray;
						if(InputHasCollided(currentTouch, out hits, out ray)) {
							// Look for the first UIControl in hits
							int hit = GetFirstHitContainingUIControl(hits);
							if(hit != -1) {
								if(multitouch || GetNumTouchesOnUIControls() == 0) {
									UIControl uiControl = hits[hit].collider.gameObject.GetComponent< UIControl >();
									if(uiControl.touchesIds.Count == 0) {	//No touches, create collider plane
										uiControl.collisionPlane.SetNormalAndPosition(uiControl.transform.forward.normalized, hits[hit].point);
										uiControl.relativeHitPosition = uiControl.transform.position - hits[hit].point;
									}
									
									info.uicontrol = uiControl;
									info.inputManager = this;
									info.collPoint = hits[hit].point;
									uiControl.touchesIds.Add(currentTouch.fingerId);
									info.uicontrol.OnTouchDown(new TouchCollision(info, ray));
								}
							}
						}
					}	
					break;
				}
				
				case DeviceInput.Phase.Stationary:
					if(info.uicontrol != null) {
						#if UNITY_EDITOR
						Ray ray = inputCamera.ScreenPointToRay(currentTouch.position);
						Debug.DrawRay(ray.origin, ray.direction * 1000.0f, Color.red);
						#endif	
					}
					break;
				
				case DeviceInput.Phase.Moved: {
					if(info.uicontrol != null) {
						Ray ray;
						GetCollisionPoint(currentTouch, info.uicontrol, ref touchInfo[currentTouch.fingerId].collPoint, out ray);
						info.uicontrol.OnTouchMoved(new TouchCollision(info, ray));
					}
					break;
				}
				
				case DeviceInput.Phase.Ended: {
					if(info.uicontrol != null) {	
						infoTmp.id = info.id;
						infoTmp.inputManager = info.inputManager;
						infoTmp.uicontrol = info.uicontrol;
						infoTmp.collPoint = info.collPoint;

						info.uicontrol = null; //we need to release the TouchInfo here to prevent the case where an UIControl disables this BravoInputManager and then OnTouchCancel called 
						info.inputManager = null;

						//Clean up touch info
						infoTmp.uicontrol.touchesIds.Remove(currentTouch.fingerId);
						
						if(infoTmp.uicontrol.touchesIds.Count > 0) {
							infoTmp.uicontrol.relativeHitPosition = infoTmp.uicontrol.transform.position - BravoInputManager.touchInfo[infoTmp.uicontrol.touchesIds[0]].collPoint ;
						}
					
					
						//Check that the released has been done in the same UIControl
						Ray ray;
						if(InputHasCollided(currentTouch, out hits, out ray)) {
							RaycastHit hit = Array.Find(hits, 
								(h) => {
									return h.collider == infoTmp.uicontrol.GetComponent<Collider>();
								}
							);
						
							if(hit.collider == infoTmp.uicontrol.GetComponent<Collider>()) {
								infoTmp.uicontrol.OnTouchUp(new TouchCollision(infoTmp, ray));
							}
							else {
								infoTmp.uicontrol.OnTouchCancelled();
							}
						} else {
							infoTmp.uicontrol.OnTouchCancelled();
						}
					}
					break;
				}
			}
		}
	}
	
	private int GetNumTouchesOnUIControls() {
		int ret = 0;
		
		foreach(DeviceInput currentTouch in InputHandler.Touches) {	
			TouchInfo info = touchInfo[currentTouch.fingerId];
			
			if(info.uicontrol != null) {
				ret ++;
			}
		}
		
		return ret;
	}
	
	/// <summary>
	/// Throws a ray from the current camera and fills hits with the collision info
	/// </summary>
	/// <param name="touchInfo">
	/// touch from where ray is thrown <see cref="DeviceInput"/>
	/// </param>
	/// <param name="hits">
	/// collision info <see cref="RaycastHit[]"/>
	/// </param>
	/// <returns>
	/// True if there has been any collision, false otherwise <see cref="System.Boolean"/>
	/// </returns>
	private bool InputHasCollided (DeviceInput _touchInfo, out RaycastHit[] _hits, out Ray _ray) {		
		_ray = inputCamera.ScreenPointToRay (_touchInfo.position);
		_hits = Physics.RaycastAll(_ray, Mathf.Infinity, inputCamera.cullingMask);
		
		if(_hits.Length > 0) {
			return true;
		}
		
		return false;
	}
	

	/// <summary>
	/// Returns the first hit in hte array passed that collides with an UIControl
	/// </summary>
	/// <param name="hits">
	/// Array of hits (sorted by distance to camera?) <see cref="RaycastHit[]"/>
	/// </param>
	/// <returns>
	/// The first hit int hte array passed that collides with an UIControl <see cref="UIControl"/>
	/// </returns>
	private int GetFirstHitContainingUIControl(RaycastHit[] _hits) {		
		Array.Sort(	
			_hits, 
			(hit1, hit2) => { 
           		return (hit1.point - inputCamera.transform.position).sqrMagnitude.CompareTo((hit2.point - inputCamera.transform.position).sqrMagnitude); 
           	}
		);
	
		for(int i = 0; i < _hits.Length; ++i) {
			if(_hits[i].collider.gameObject.GetComponent< UIControl >() != null)
				return i;
		}
		
		return -1;
	}
	
	
	private bool GetCollisionPoint(DeviceInput _deviceInput, UIControl _uiControl, ref Vector3 _point, out Ray _ray) {
		float dist;
		_ray = inputCamera.ScreenPointToRay(_deviceInput.position);
		
		#if UNITY_EDITOR
			Debug.DrawRay(_ray.origin, _ray.direction * 1000.0f, Color.red);
		#endif
		
		if(_uiControl.collisionPlane.Raycast(_ray, out dist)) {
			_point = _ray.GetPoint(dist);
			return true;
		}
		return false;
	}
}
