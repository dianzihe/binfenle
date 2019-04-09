using UnityEngine;
using System.Collections;
using System.Linq;

public class LevelButtons : MonoBehaviour {

	public void Init(bool forceRedraw = false)
	{
		// projects all children to the book
		GetComponent<ChildrenProjector>().ProjectChildren();
		// refresh buttons info (locked/unlocked)
		RefreshLevelsInfo();
		bool levelsAnimationIsNeeded = FireAnimations.Instance.UpdateAvatar(forceRedraw);
		// the only case the path will not be drawn when entering it's in case of unlocked level animation
		if(!levelsAnimationIsNeeded)
			GetComponent<PathDrawer>().DrawPath(forceRedraw);
	}

	// this function is called every time that the game info may suffer a change
	public void RefreshLevelsInfo()
	{
		// reset avatar instance
		FireAnimations.Instance.UpdateFirePosition(null);

		LoadLevelButton loadLevelScript = null;
		foreach(Transform child in transform.Cast<Transform>().OrderBy(t=>t.name))
		{
			loadLevelScript = child.GetComponent<LoadLevelButton>();
			if(loadLevelScript != null)
				loadLevelScript.UpdateButtonStatus();
		}
	}
}
