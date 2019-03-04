using UnityEngine;
using System.Collections;

public class BottomCollider : MonoBehaviour
{
	private static BottomCollider instance;
	protected WaitForEndOfFrame waitEndOfFrame;
	
	public Camera renderCamera;
	
	public static BottomCollider Instance {
		get {
			return instance;
		}
	}
	
	public float PosY {
		get {
			return posY;
		}
	}
	
	protected float posY;
	
	[System.NonSerialized]
	public Transform cachedTransform;
	
	
	void Awake()
	{
		instance = this;
		cachedTransform = transform;
		waitEndOfFrame = new WaitForEndOfFrame();
		
		if (Screen.width >= Screen.height) {
			OrientationChanged(ScreenOrientation.Landscape);
		}
		else {
			OrientationChanged(ScreenOrientation.Portrait);
		}
	}
	
	void Start()
	{
		OrientationListener.Instance.OnOrientationChanged += OrientationChanged;
	}
	
	void OrientationChanged(ScreenOrientation newOrientation)
	{
		StartCoroutine(UpdatePos());
	}
	
	IEnumerator UpdatePos()
	{
		yield return waitEndOfFrame;
		
		Vector3 newPos = cachedTransform.position;
		newPos.y = renderCamera.ViewportToWorldPoint(new Vector3(0f, 0.002f, 0f)).y;
		cachedTransform.position = newPos;
		
		posY = newPos.y;
	}
}

