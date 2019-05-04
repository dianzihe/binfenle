using UnityEngine;
using System.Collections;

public class ContinueBtnController : MonoBehaviour 
{
	public NumberPickerController agePicker;
	
	void Awake()
	{
		agePicker.OnSelectionChanged += OnNumberSelectionChanged;
	}
	
	// Use this for initialization
	void Start () 
	{
		UpdateButtonState(agePicker);
	}

	public void UpdateButtonState(NumberPickerController _agePicker)
	{
		// Check if age picker has a valid selection.
		if (_agePicker.SelectedNumber < 0) {
			gameObject.SetActive(false);
		}
		else {
			gameObject.SetActive(true);
		}
	}
	
	void OnNumberSelectionChanged(NumberPickerController _agePicker)
	{
		UpdateButtonState(_agePicker);
	}
	
	void OnDestroy()
	{
		agePicker.OnSelectionChanged -= OnNumberSelectionChanged;
	}
}
