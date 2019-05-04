using UnityEngine;
using System.Collections;

public class TutrialAddManager : MonoBehaviour 
{
	void UseManaInTutrial()
	{

		switch(MaleficentBlackboard.Instance.level)
		{
		case 6:

			CostItemManager.mInstance.AddCount(1,"Crow_Prop_Count");
			break;
		case 7:

			CostItemManager.mInstance.AddCount(1,"WingWindCost_Prop_Count");
			break;
		case 8:

			CostItemManager.mInstance.AddCount(1,"YellowPixieDustCost_Prop_Count");
			break;
		case 17:

			CostItemManager.mInstance.AddCount(1,"GreenMagicCost_Prop_Count");
			break;
		case 32:

			CostItemManager.mInstance.AddCount(1,"TheStaffCost_Prop_Count");
			break;
		case 47:

			CostItemManager.mInstance.AddCount(1,"Crow2nd_Prop_Count");
			break;
		case 62:

			CostItemManager.mInstance.AddCount(1,"ThorwnCost_Prop_Count");
			break;
		case 92:

			CostItemManager.mInstance.AddCount(1,"WolfHowlCost_Prop_Count");
			break;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
