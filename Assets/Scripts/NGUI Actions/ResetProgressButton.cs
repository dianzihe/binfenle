using UnityEngine;
using System.Collections;

public class ResetProgressButton : MonoBehaviour 
{
	void OnClick()
	{
		//User.CurrentUser.ResetUserInfo();
		UserManagerCloud.Instance.ResetLocalUser();
	}
}
