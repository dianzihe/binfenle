using UnityEngine;
using System.Collections;

public class ArrowBlock : InputBlock
{
	[System.NonSerialized]
	public Component cachedRender;
	[System.NonSerialized]
	public RenderWrapper cachedRenderWrapper;
	[System.NonSerialized]
	public Component cachedInputReceiver;
	[System.NonSerialized]
	public InputWrapper cachedInputWrapper;
		
	/// <summary>
	/// Unity method.
	/// Start this instance.
	/// </summary>
	public override void Start()
	{
		base.Start();
		
		GenericSettings.Instance.OnAutoplayChanged += OnSettingChanged;
		GenericSettings.Instance.OnArrowsChanged += OnSettingChanged;
		
		if (children.Count == 0) {
			cachedRenderWrapper = RenderWrapper.Instance;
			cachedRender = cachedRenderWrapper.GetRenderComponent(gameObject);
			
			cachedInputWrapper = InputWrapper.Instance;
			cachedInputReceiver = cachedInputWrapper.GetInputComponent(gameObject);
		}
		
		UpdatedState();
	}

	protected void OnSettingChanged(bool on)
	{
		UpdatedState();
	}
	
	protected void UpdatedState()
	{
		gameObject.SetActive(!GenericSettings.Instance.Autoplay);
		
		if (cachedRender) {
			cachedRenderWrapper.SetEnabled(cachedRender, GenericSettings.Instance.Arrows);
			cachedInputWrapper.SetEnabled(cachedInputReceiver, GenericSettings.Instance.Arrows);
		}
	}
}

