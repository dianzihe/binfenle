using UnityEngine;
using System.Collections;

public class LivesLabel : MonoBehaviour 
{
	protected UILabel myLabel;
	float livesLabelX;
	bool _posYmodified;

	void Awake() 
	{
		myLabel = GetComponent<UILabel>();
		livesLabelX = myLabel.transform.localPosition.x;
	}
	
	void Start()
	{
		UpdateText();
		LivesSystem.OnLivesUpdate += UpdateText;
	}
	
	void UpdateText() 
	{
		Vector3 livesLabelPos = myLabel.transform.localPosition;

		float posY = livesLabelPos.y;
		if(_posYmodified){
			_posYmodified = false;
			posY = livesLabelPos.y + 10f;
		}

		if(LivesSystem.lives > 0 && LivesSystem.lives < LivesSystem.maxLives)
		{
			myLabel.text = LivesSystem.lives.ToString();
			myLabel.transform.localPosition = new Vector3(livesLabelX + 74f,posY,livesLabelPos.z);
		}
		else if(LivesSystem.lives == 0)
		{
			myLabel.text = "+";
			myLabel.transform.localPosition = new Vector3(livesLabelX + 90f,posY - 10f,livesLabelPos.z);
			_posYmodified = true;
		}
		else
		{
			myLabel.text = LivesSystem.lives.ToString();
			myLabel.transform.localPosition = new Vector3(livesLabelX,posY,livesLabelPos.z);
		}
	}
	
	void OnDestroy()
	{
		LivesSystem.OnLivesUpdate -= UpdateText;
	}
}
