using UnityEngine;
using System.Collections;

public class ManaAddWidget : MonoBehaviour {

	void Start () 
	{
		if(!MaleficentTools.IsDebugBuild) {
			Destroy (gameObject);
		}
	}

	void OnClick ()
	{
		TokensSystem.Instance.AddMana(100);
		SoundManager.Instance.PlayOneShot("mana_earn_sfx");
	}
}
