using UnityEngine;
using System.Collections;

public class ItemDescriptionLabel : MonoBehaviour 
{
	public static string[] descriptions = new string[]{
		"TORCH_DESCRIPTION", 
		"GLACIER_DESCRIPTION", 
		"CARROT_DESCRIPTION", 
		"ICE_AXE_DESCRIPTION", 
		"HOT_CHOCOLATE_DESCRIPTION", 
		"TROLL_MAGIC_DESCRIPTION", 
		"SWORD_DESCRIPTION", 
		"TORCH_DESCRIPTION", 
		"GLACIER_DESCRIPTION", 
		""
	};
	
	public AudioClip playSoundIn;
	public AudioClip playSoundOut;
	
	public ItemDescriptionLabel twinDescription;

	protected UILabel myLabel;
	protected UITexture myBackground;
	protected GameObject daddy;
	
	[System.NonSerialized]
	public bool shown = false;
	
	void Awake()
	{
		myLabel = GetComponent<UILabel>();
		
		daddy = transform.parent.gameObject;
		myBackground = transform.parent.Find("Background").GetComponent<UITexture>();
	}
	
	public void UpdateDescription(int index) 
	{
		if (myLabel == null) {
			return;
		}
		
		if (index < 0 || index >= descriptions.Length || descriptions[index] == "") {
			myLabel.text = "";
			myBackground.enabled = false;
		}
		else {
			myLabel.text = Language.Get(descriptions[index]);
			myBackground.enabled = true;
		}
	}
	
	public void Show()
	{			
		if (!shown) {
			shown = true;
			
			if (myLabel.text != "")
			{
				myLabel.enabled = true;
				myBackground.enabled = true;
			}
			
			if (!twinDescription.shown) {
				twinDescription.Show();
			}
			
			daddy.GetComponent<Animation>().Play("ScaleUp");
			
			if (playSoundIn != null) {
				NGUITools.PlaySound(playSoundIn);
			}
		}
	}
	
	void OnClick()
	{		
		Hide();
	}
	
	public void Hide()
	{
//		Debug.Log("Hide" + transform.parent.parent.parent.name);
		if (shown) {
			shown = false;
			
			if (twinDescription.shown) {
				twinDescription.Hide();
			}
			
			daddy.GetComponent<Animation>().Play("ScaleDown");
		
			if (playSoundOut != null) {
				NGUITools.PlaySound(playSoundOut);
			}
		}
		else {
			transform.parent.localPosition = new Vector3(0, -4000f, 0f);
			myLabel.enabled = false;
			myBackground.enabled = false;
		}
	}
}
