using UnityEngine;
using System.Collections;

public class Chalinger : MonoBehaviour
{
	public static Chalinger mIns=null;
	public static Chalinger getInstance()
	{
		return mIns;
	}
	void Awake()
	{
		mIns=this;
	}
	public void AddLevelChanTimes(string strLev)
	{
		int nRet=0;
		nRet=PlayerPrefs.GetInt("MaleficentLevel_"+strLev,-1);
		if(nRet==-1)
		{
			PlayerPrefs.SetInt("MaleficentLevel_"+strLev,1);
		}
		else
		{
			PlayerPrefs.SetInt("MaleficentLevel_"+strLev,nRet+1);
		}
		PlayerPrefs.Save();
	}
	public int GetLevelChanTimes(string strLevelName)
	{
		return PlayerPrefs.GetInt("MaleficentLevel_"+strLevelName);
	}
	public void DelLevelChanTimes(string strLev)
	{

	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
