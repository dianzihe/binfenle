using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Number selection controller must be placed on the UIDragPanelContents gameobject and it must have at least one
/// child UILabel already as a template from which it will instantiate the other labels for the specified number intervals. [minNumber - maxNumber]
/// 
/// Raised Unity message events:
/// "OnNumberSelectionChanged(NumberPickerController sender);
/// 
/// </summary>
public class NumberPickerController : MonoBehaviour
{
	public const string unitySelectionChangedEventName = "OnNumberSelectionChanged";
		
	public event System.Action<NumberPickerController> OnSelectionChanged;
	
	public NumberPickerController twinComponent;
		
	#region Editor properties
	[SerializeField]
	protected int minNumber = 3;
	[SerializeField]
	protected int maxNumber = 99;

	public float spaceBetweenLabels = 0f;
	
	public int initialSelectedIdx = 0;
	#endregion
	
	#region Editor events properties
	public GameObject eventTarget;
	#endregion
	
	[System.NonSerialized]
	public bool isInitialized = false;
	
	[System.NonSerialized]
	public List<UILabel> uiNumberLabels;
	
//	[SerializeField]
	protected int selectedNumber;
	
	protected Transform firstLabelTransform;
		
	protected UICenterOnChild centerOnChild;
	protected UIDragScrollView  draggablePanel;
	
	[System.NonSerialized]
	public BoxCollider cachedCollider;
	
	[System.NonSerialized]
	public Transform cachedTransform;	


	#region MonoBehavior init
	// Use this for initialization
	void Awake()
	{
		cachedTransform = transform;
		cachedCollider = GetComponent<Collider>() as BoxCollider;
		
		centerOnChild = GetComponent<UICenterOnChild>();
		draggablePanel = NGUITools.FindInParents<UIDragScrollView >(gameObject);
		
		centerOnChild.onFinished = OnFinishedSnap;		
	}

	void Start()
	{
		int initialCapacity = Mathf.Clamp(maxNumber - minNumber + 2, 10, 5000);
		uiNumberLabels = new List<UILabel>(initialCapacity);

		RebuildNumberPicker();
		
//		isInitialized = true;

		FocusItem(initialSelectedIdx);
	}
	#endregion
	
	#region Widget properties & methods
	/// <summary>
	/// Gets or sets the selected number in this number picker widget.
	/// </summary>
	/// <value>
	/// The selected number.
	/// </value>
	public int SelectedNumber 
	{
		get {
			return selectedNumber;
		}
		
		protected set
		{
			int oldValue = selectedNumber;
			selectedNumber = value;
			
			if (selectedNumber != oldValue) {
				RaiseSelectionChangedEvent();
			}
		}
	}
		
	public int MinNumber
	{
		get {
			return minNumber;
		}
		set {
			minNumber = value;
		}
	}
	
	public int MaxNumber
	{
		get {
			return maxNumber;
		}
		set {
			maxNumber = value;
		}
	}	

	/// <summary>
	/// Returns the UILabel index (from the uiNumberLabels list) corresponding to the specified number.
	/// </summary>
	/// <returns>
	/// The corresponding label index.
	/// </returns>
	/// <param name='number'>
	/// The specified number for which you want the corresponding UILabel index in the "uiNumberLabels" list. 
	/// </param>
	public int NumberToLabelIdx(int number)
	{	
		// The "+1" is because the first UILabel is not a number it's just a symbol indicating no item was selected (ex: "-"), so the numbered labels start
		// from the next index.
		return Mathf.Clamp(number, minNumber, maxNumber) - minNumber + 1;
	}
		
	public void ScrollToItemIdx(int itemIndex)
	{
		GameObject targetItem = uiNumberLabels[itemIndex].gameObject;
		//centerOnChild.centeredObject = targetItem;
		//centerOnChild.Recenter(centerOnChild.centeredObject);
	}
	
	public void FocusLastItem()
	{
		FocusItem(uiNumberLabels.Count - 1);
	}
	
	public void FocusItem(int itemIndex)
	{
		/* 
		// Pick the target item from with given index
		GameObject targetItemGO = uiNumberLabels[itemIndex].gameObject;
		//centerOnChild.centeredObject = targetItemGO;
		Transform targetItemTransform = targetItemGO.transform;
		
		Transform dt = draggablePanel.panel.cachedTransform;
		Vector3 center = dt.localPosition;
		Vector4 clip = draggablePanel.panel.clipRange;
		center.x += clip.x;
		center.y += clip.y;
		center = dt.parent.TransformPoint(center);

		// Figure out the difference between the chosen child and the panel's center in local coordinates
		Vector3 offset = dt.InverseTransformPoint(targetItemTransform.position) - dt.InverseTransformPoint(center);
		
		// Offset shouldn't occur if blocked by a zeroed-out scale
		if (draggablePanel.scale.x == 0f) offset.x = 0f;
		if (draggablePanel.scale.y == 0f) offset.y = 0f;
		if (draggablePanel.scale.z == 0f) offset.z = 0f;
		
		Vector3 targetPos = dt.localPosition - offset;
		Vector3 prevPos = dt.localPosition;
		
		dt.localPosition = targetPos;
		offset = targetPos - prevPos;
		Vector4 cr = draggablePanel.panel.clipRange;
		cr.x -= offset.x;
		cr.y -= offset.y;
		draggablePanel.panel.clipRange = cr;
		
		draggablePanel.panel.Refresh();
		
		OnFinishedSnap();
		*/
	}
	#endregion
	
	#region Widget initialization
	/// <summary>
	/// Inits the number picker widget.
	/// Must be manually called after changing the "minNumber" and "maxNumber" to refresh the widget content.
	/// </summary>
	public void RebuildNumberPicker()
	{
		int lastLabelsCount = uiNumberLabels.Count;
		
		// Remove old number labels from the list (easier than to re-use the old ones) except for the first one that is used as a template.
		for (int i = 1; i < uiNumberLabels.Count; i++) {
			GameObject.Destroy(uiNumberLabels[i].gameObject);
		}
		
		// Clear the old UILabels from the list except the first one which is used as a template for the new labels.
		if (uiNumberLabels.Count > 1) {
			uiNumberLabels.RemoveRange(1, uiNumberLabels.Count - 1);
		}
		
		GenerateNumberLabels();
		RefreshScrollCollider();
		centerOnChild.Recenter();
		
		// Collect old garbaged memory in case of widget re-init (where we destroy all UILabels and re-alloc new ones).
		if (lastLabelsCount > 1) { 
			System.GC.Collect();
		}		
	}
		
	/// <summary>
	/// Generates the number labels for this widget.
	/// </summary>
	protected void GenerateNumberLabels()
	{
		if (cachedTransform.childCount == 0 || cachedTransform.childCount > 1)
		{
			Debug.LogError("[NumberSelectionController] Requires one child UILabel as a template for instantiating the other labels!");
		}
		else 
		{
			firstLabelTransform = cachedTransform.GetChild(0);
			UILabel uiLabel = firstLabelTransform.GetComponent<UILabel>();
			
			if (uiLabel != null) 
			{
				uiLabel.name = uiLabel.text;
				uiNumberLabels.Add(uiLabel);
			}
			else {
				Debug.LogError("[NumberSelectionController] UILabel component required on the first child gameobject!");
			}
		}

		Transform lastTransform = uiNumberLabels[0].transform;
		
		for(int number = minNumber; number <= maxNumber; number++)
		{
			int labelIdx = NumberToLabelIdx(number);

			GameObject newLabelGO = Instantiate(uiNumberLabels[labelIdx - 1].gameObject) as GameObject;
			newLabelGO.name = number.ToString();
			Transform newLabelTransform = newLabelGO.transform;
			newLabelTransform.parent = cachedTransform;
			newLabelTransform.localScale = lastTransform.localScale;

			// Cache the new label.
			UILabel newLabel = newLabelGO.GetComponent<UILabel>();
			newLabel.text = newLabelGO.name;
			newLabel.panel.Refresh();
			uiNumberLabels.Add( newLabel );
			// Reposition the new label
			newLabelTransform.localPosition = lastTransform.localPosition + Vector3.down * (lastTransform.localScale.y + spaceBetweenLabels);

			lastTransform = newLabelTransform;
		}
	}
	
	/// <summary>
	/// Re-scales and repositions the scrolling collider (that triggers the drag input) to encapsulate all the UILabels in this widget.
	/// </summary>
	protected void RefreshScrollCollider()
	{
		float newBoxHeight = (firstLabelTransform.localScale.y + spaceBetweenLabels) * uiNumberLabels.Count;
		
		Vector3 colliderSize = cachedCollider.size;
		colliderSize.y = newBoxHeight;
		cachedCollider.size = colliderSize;
		
		Vector3 colliderCenter = cachedCollider.center;
		colliderCenter.y = -newBoxHeight * 0.5f - colliderCenter.y;
		cachedCollider.center = colliderCenter;
	}
	#endregion
	
	#region Widget events related
	public void RaiseSelectionChangedEvent()
	{
		if (OnSelectionChanged != null) {
			OnSelectionChanged(this);
		}
		
		if (eventTarget != null) {
			eventTarget.SendMessage(unitySelectionChangedEventName, this, SendMessageOptions.RequireReceiver);
		}
	}
	
	/// <summary>
	/// NGUI event from UICenterOnChild component.
	/// </summary>
	public void OnFinishedSnap()
	{	
		int newSelectedValue = 0;
		if ( !int.TryParse(centerOnChild.centeredObject.name, out newSelectedValue) ) 
		{
			newSelectedValue = -1;
		}
		
		SelectedNumber = newSelectedValue;
		
		if (twinComponent != null)
		{
			// Sync the twin component with this current one
			twinComponent.SelectedNumber = SelectedNumber;

			//twinComponent.draggablePanel.panel.clipRange = draggablePanel.panel.clipRange;
			//twinComponent.draggablePanel.panel.cachedTransform.localPosition = draggablePanel.panel.cachedTransform.localPosition;
			// twinComponent.draggablePanel.panel.Refresh();
		}
	}
	
	#endregion
}
