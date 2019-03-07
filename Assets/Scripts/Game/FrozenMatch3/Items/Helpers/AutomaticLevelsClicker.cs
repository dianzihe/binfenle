using UnityEngine;
using System.Collections;

public class AutomaticLevelsClicker : MonoBehaviour {
	
	public static AutomaticLevelsClicker instance;
	
	static int levelButtonIndex = 2;
	static int iterationCount = 0;
	
	public float waitTimeBetweenActions = 2f;
	
	public int iterationsPerLevel = 2;
	
	public bool automaticallyExitLevels = true;
	
	protected bool enteredLevel = false;
	
	void Awake()
	{
		if(instance == null)
		{
			GameObject.DontDestroyOnLoad(gameObject);
			instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}
	
	void Start()
	{
		Match3BoardGameLogic.OnStartGame += OnStartGame;
		StartCoroutine(IterateTroughLevels());
	}
	
	private IEnumerator IterateTroughLevels()
	{
		while(true)
		{
			yield return new WaitForSeconds(waitTimeBetweenActions);
			
			if(Application.loadedLevelName == "Map")
			{
				if(enteredLevel)
				{
					Debug.LogError("Quit unexpectedly from Level " + levelButtonIndex / 2);
				}
				
				if(iterationCount >= iterationsPerLevel)
				{
					levelButtonIndex+=2;
					iterationCount = 0;
				}
			
/*				if(levelButtonIndex >= CompanionsManager.Instance.levelButtons.Length)
				{
					Debug.LogWarning("[AutomaticLevelsClicker] All done!");
					yield break;
				}

				Debug.LogWarning("[AutomaticLevelsClicker] Length:" + CompanionsManager.Instance.levelButtons.Length);

				CompanionsManager.Instance.levelButtons[levelButtonIndex].gameObject.SendMessage("OnClick");
	*/			
				GameObject playButton = GameObject.Find("UI Root Landscape/Camera/Anchor/Scaler/Level Panel Landscape/Play Button");
				
				yield return new WaitForSeconds(waitTimeBetweenActions);
				
				playButton.SendMessage("OnClick");
				enteredLevel = true;
				iterationCount++;
			}
		}
	}
	
	void OnStartGame()
	{		
		if(automaticallyExitLevels)
		{
			ExitGame();
		}
	}
	
	public void ExitGame()
	{
		StartCoroutine(ExitGameCoroutine());
	}
	
	IEnumerator ExitGameCoroutine()
	{
		yield return new WaitForSeconds(waitTimeBetweenActions);
		
		GameObject pauseButton = GameObject.Find("UI Root Landscape/Camera/Anchor/Main Panel/GUI Anchor/Pause Button");
		pauseButton.SendMessage("OnClick");
		
		yield return new WaitForSeconds(waitTimeBetweenActions);
		
		GameObject exitButton = GameObject.Find("UI Root Landscape/Camera/Anchor/Scaler/Pause Panel Landscape/Controls/Grid/02 Exit Button");
		exitButton.SendMessage("OnClick");
		enteredLevel = false;
	}
}
