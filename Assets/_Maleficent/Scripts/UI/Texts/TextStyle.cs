using UnityEngine;
using System.Collections;

public class TextStyle : MonoBehaviour {

	public string styleName = "";

	private UILabel _uilabel;

	private TextStyleManager.Style _style;
	private TextStyleManager.Style style {
		get { 
			if (_style == null) {
				TextStyleManager.Style style = TextStyleManager.Instance.GetStyleWithName(styleName);
				_style = style;
			}
			return _style;
		}
	}

	void Awake()
	{
		_uilabel = gameObject.GetComponent<UILabel>();

		if(_uilabel == null || style == null) {
			Destroy(this);
		}else if (style != null) {
			style.OnStyleChanged += HandleOnStyleChanged;
		}
	}

	void Start()
	{
		_UpdateStyle();
	}

	void HandleOnStyleChanged (TextStyleManager.Style style)
	{
		_UpdateStyle();
	}

	void _UpdateStyle()
	{
		_uilabel.color = style.textColor;
		_uilabel.effectColor = style.effectColor;
		_uilabel.effectStyle = style.effectStyle;
		_uilabel.effectDistance = style.effectDistance;
	}
}
