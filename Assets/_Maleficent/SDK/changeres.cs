﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.IO;
using System.IO.Compression;

public class changeres : MonoBehaviour {
	public string HandResultCode;
	public string PlatformResult;
	public string[] ChangeArray;
	public bool isright=false;
	void Start (){
		DontDestroyOnLoad (gameObject);
	}
	void Handler_POST_CHANGECODE(string resultcode)
	{
		HandResultCode = resultcode;
		Debug.Log("HandResultCode="+HandResultCode);
		ChangeArray=HandResultCode.Split(';');
		if (ChangeArray [0] == "200") {
						int[] changeCount = new int[14];
			
						for (int i=1; i<ChangeArray.Length; i++) {
								string[] CCode = ChangeArray [i].Split (',');
								if (CCode [0] == "1") {
										changeCount [0] = 40 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "2") {
										changeCount [1] = 70 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "3") {
										changeCount [2] = 100 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "4") {
										changeCount [3] = 130 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "5") {
										changeCount [4] = 200 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "6") {
										changeCount [5] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "7") {
										changeCount [6] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "8") {
										changeCount [7] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "9") {
										changeCount [8] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "10") {
										changeCount [9] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "11") {
										changeCount [10] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "12") {
										changeCount [11] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "13") {
										changeCount [12] = 1 * Convert.ToInt32 (CCode [1]);
								}
								if (CCode [0] == "14") {
										changeCount [13] = 1 * Convert.ToInt32 (CCode [1]);
								}

						}
						int SumCount = changeCount [0] + changeCount [1] + changeCount [2] + changeCount [3] + changeCount [4];
						int LiveCount = changeCount [5];
						string showlive="";
						if(LiveCount>0)
						{
							showlive="且增加生命值"+LiveCount+"点。";
						}
						string showmofanum="";
						if(SumCount>0){
							showmofanum="魔法值"+SumCount+"点。";
						}
						print ("SumCount= " + SumCount);
						TokensSystem.Instance.AddMana (SumCount);
						LivesSystem.lives+= LiveCount;
						string[] showdaoju=new string[8];
						string showget="";
						string[] daojuname={"乌鸦魔法","恶魔乌鸦魔法","权杖魔法","绿雾魔法","风之翼魔法","金风魔法","狼魔法","荆棘丛生魔法"};
						CostItemManager.mInstance.AddCount (changeCount [6], "Crow_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [7], "Crow2nd_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [8], "TheStaffCost_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [9], "GreenMagicCost_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [10], "WingWindCost_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [11], "YellowPixieDustCost_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [12], "WolfHowlCost_Prop_Count");
						CostItemManager.mInstance.AddCount (changeCount [13], "ThorwnCost_Prop_Count");
						for(int i=6;i<changeCount.Length;i++){
							if(changeCount[i]!=0){
								 showdaoju[i-6]=daojuname[i-6]+changeCount[i]+"个、";
								 showget+=showdaoju[i-6];
							}
						}
						
						string strDesc = Language.Get ("CHANGE_CODE_SUCCESS");
						string strDesc2 = strDesc + showget.ToString() +showmofanum.ToString()+showlive.ToString();
						if (SDKTipsWindowController.getInstance () != null) 
						{
								SDKTipsWindowController.getInstance ().PopWindow (strDesc2, Hand_ChangeCode_OK_CallBack, null);
						}
			
			
				} else if (ChangeArray [0] == "201") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_NOT_EXIST");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
						}
				} else if (ChangeArray [0] == "202") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_HAVE_USED");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
						}
				} else if (ChangeArray [0] == "203") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_TIMEOUT");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
						}
				} else if (ChangeArray [0] == "204") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_PACKAGE");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
						}
				} else if (ChangeArray [0] == "205") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_NULL");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
						}
				} else if (ChangeArray [0] == "206") {
						string strDesc = Language.Get ("CHANGE_CODE_WRONG_USER_USED");
						if (SDKTipsWindowController.getInstance () != null) {
				SDKTipsWindowController.getInstance ().PopWindow (strDesc, Hand_ChangeCode_OK_CallBack, null);
			}
		}
	}
	void Handler_Platform_Check(string platformcheck)
	{
		PlatformResult = platformcheck;
		Debug.Log ("PlatformResult=" + PlatformResult);

	}
	void Hand_ChangeCode_OK_CallBack(){
		isright=true;
	}
	void Hand_ChangeCode_Cancel_CallBack(){
		isright = false;
	}

}