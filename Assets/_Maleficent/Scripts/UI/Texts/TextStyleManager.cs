using UnityEngine;
using System.Collections;

public class TextStyleManager : MonoBehaviour {

	[System.Serializable]
	public class Style {

		public delegate void StyleChangedHandler(Style style);
		public event StyleChangedHandler OnStyleChanged;

		public string name;
		public Color textColor;
		public UILabel.Effect effectStyle;
		public Color effectColor;
		public Vector2 effectDistance;

		public void NotifyOnStyleChanged()
		{
			if (OnStyleChanged != null) {
				OnStyleChanged(this);
			}
		}
	}
	public Style[] styles;
	static TextStyleManager _instance = null;
	public static TextStyleManager Instance {
		get {
			if(_instance == null) {
				GameObject textStyleManager = (Instantiate(Resources.Load("TextStyleManager")) as GameObject);
				if(textStyleManager == null) {
					Debug.Log("Text manager prefab not found");
				} else {
					textStyleManager.name = "TextStyleManager";
					_instance = textStyleManager.GetComponent<TextStyleManager>();
				}
			} 
			
			return _instance;
		}
	}

	public void NotifyStyleChange()
	{
		foreach (Style style in styles) {
			style.NotifyOnStyleChanged();
		}
	}

#region Public methods

	public Style GetStyleWithName(string name)
	{
		Style result = null;
		foreach (Style item in styles) {
			if (item.name == name) {
				result = item;
				break;
			}
		}

		if (result == null) {
			Debug.Log("[TextStyleManager] Style not found: " + name);
		}

		return result;
	}

#endregion

}
