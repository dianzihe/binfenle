using UnityEngine;
using System.Collections;

public class MaleficentBlackboard : MonoBehaviour {

	private static MaleficentBlackboard instance;
	public static MaleficentBlackboard Instance {
		get {
			if(instance == null) {
				GameObject blackboard = Blackboard.Instance;
				if(blackboard != null) {
					instance = blackboard.GetComponent< MaleficentBlackboard >();
				}
			}

			return instance;
		}
	}

	public int character;
	public int level;
	public int levelBg;
	public bool unlockAllItems;

}
