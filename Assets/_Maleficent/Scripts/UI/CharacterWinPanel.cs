using UnityEngine;
using System.Collections;

public class CharacterWinPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<UISprite>().spriteName = CompanionSelect.iconPrefix + Match3BoardGameLogic.Instance.characterUsed;
	}
}
