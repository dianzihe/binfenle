using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net.Sockets;
using System.IO;
using System.Net;
public class SigninSystem : MonoBehaviour {
	public UIEventListener signclick=null;
	public GameObject qd1;
	public GameObject qd2;
	public UILabel labeltext;
	public UILabel signtext0;
	public UILabel signtext1;
	public UILabel signtext2;
	public UILabel signtext3;
	string SignTime="01/01/2010";
	string DayTime;
	string TimeRead;
	string[] SignTimeArray;
	string[] DayTimeArray;
	public GameObject redObj;
	public bool issign=false;
	public int count;
	string[] MonthArray;
	string ip;
	string port;
	string URLPath="http://maleficent.zooking.cn:8080/SleepCurse/systemTime";
	//public GameObject SignObj;
	//string URLPath="http://192.168.50.36/";
	public GameObject clickObjfront;
	public GameObject clickObjback;
	Dictionary<int,string> DayRecord=new Dictionary<int, string>();
	Dictionary<int,string> Reward=new Dictionary<int, string>();
	int SumCount;
	public bool isigned;
	string[] RewardArray;
	private float mJindu = 0;
	public static SigninSystem instanse=null;
	public static SigninSystem getinstanse()
	{
		return instanse;
	}
	void Awake()
	{
		
	}
	// Use this for initialization
	void Start () {
		instanse = this;
		SignTimeArray=SignTime.Split('/');
		DayRecord.Add(1,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(2,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28");
		DayRecord.Add(3,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(4,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30");
		DayRecord.Add(5,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(6,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30");
		DayRecord.Add(7,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(8,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(9,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30");
		DayRecord.Add(10,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		DayRecord.Add(11,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30");
		DayRecord.Add(12,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31");
		Reward.Add(1,"10");
		Reward.Add(2,"10");
		Reward.Add(3,"10");
		Reward.Add(4,"10");
		Reward.Add(5,"10");
		Reward.Add(6,"50,2,0");
		Reward.Add(7,"10");
		Reward.Add(8,"15");
		Reward.Add(9,"15");
		Reward.Add(10,"15");
		Reward.Add(11,"60,2,5");
		Reward.Add(12,"15");
		Reward.Add(13,"15");
		Reward.Add(14,"15");
		Reward.Add(15,"20");
		Reward.Add(16,"20");
		Reward.Add(17,"20");
		Reward.Add(18,"20");
		Reward.Add(19,"20");
		Reward.Add(20,"20");
		Reward.Add(21, "70,2,6");
		Reward.Add(22,"25");
		Reward.Add(23,"25");
		Reward.Add(24,"25");
		Reward.Add(25,"25");
		Reward.Add(26,"25");
		Reward.Add(27,"25");
		Reward.Add(28,"25");
		Reward.Add(29,"80,2,4");
		Reward.Add(30,"68");
		Reward.Add(31,"70");
		
		StartCoroutine (POST ());
		
	}
	
	// Update is called once per frame
	void Update () {
		System.DateTime currentTime = System.DateTime.Now;
		DayTime=currentTime.ToString("d");
		//DayTimeArray=DayTime.Split('/');
		//		TimeRead = SignServerReadText.instanse.TimeResult;

		//int month = Convert.ToInt32 (DayTimeArray [0]);

		if (signclick != null) {
			signclick.onClick=OnSignClick;
		}
		
	}
	void RefreshData()
	{
		DayTimeArray=TimeRead.Split('-');
		int month = Convert.ToInt32 (DayTimeArray [1]);
		if (Convert.ToInt32 (DayTimeArray [0]) % 4 == 0) {
			DayRecord.Add(2,"1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29");	
			DayRecord[2]="1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29";
		}
		string Datet = DayRecord [month];
		MonthArray=Datet.Split(',');
		for (int i=0; i<MonthArray.Length; i++) {
			if(Convert.ToInt32 (DayTimeArray [2])==Convert.ToInt32(MonthArray[i]))
			{
				labeltext.text=(MonthArray.Length-Convert.ToInt32 (DayTimeArray [2])).ToString();
			}
		}
		DateTime dt = DateTime.Parse(PlayerPrefs.GetString ("SignTime","2010-01-01"));
		TimeSpan ts = DateTime.Parse(TimeRead) - dt;
		//TimeSpan ts = System.DateTime.Now - dt;
		if (ts.TotalMinutes  > 0 && dt.Day != DateTime.Parse(TimeRead).Day) 
			//		if (ts.TotalMinutes  > 0 && dt.Day != DateTime.Parse(TimeRead).Day) 
		{
			//
			PlayerPrefs.SetString ("SignTime", TimeRead);
			qd1.SetActive (true);
			qd2.SetActive (false);
			redObj.SetActive (true);
			for(int i=1;i<=PlayerPrefs.GetInt("count");i++)
			{
				clickObjback.transform.Find ("signbg"+i).GetComponent<UISprite> ().enabled = false;
				clickObjfront.transform.Find ("signbg"+i).GetComponent<UISprite> ().enabled = true;
			}
			issign=false;
			PlayerPrefs.SetString("issign",issign.ToString());
		}
		
		if ((int)ts.TotalDays == 0) {
			//PlayerPrefs.SetString ("SignTime", DateTime.Now.ToString());
			if(bool.Parse(PlayerPrefs.GetString("issign")))
			{
				qd1.SetActive(false);
				qd2.SetActive(true);
				redObj.SetActive(false);
				
				for(int i=1;i<=PlayerPrefs.GetInt("count");i++)
				{
					clickObjback.transform.Find("signbg"+i).GetComponent<UISprite>().enabled=true;
					clickObjfront.transform.Find("signbg"+i).GetComponent<UISprite>().enabled=false;
				}
				
			}else
			{
				qd1.SetActive (true);
				qd2.SetActive (false);
				redObj.SetActive (true);
				for(int i=1;i<=PlayerPrefs.GetInt("count");i++)
				{
					clickObjback.transform.Find("signbg"+i).GetComponent<UISprite>().enabled=true;
					clickObjfront.transform.Find("signbg"+i).GetComponent<UISprite>().enabled=false;
				}
			}
			
		}


		if (MonthArray.Length - Convert.ToInt32 (DayTimeArray [2]) == 0) {
			for(int i=1;i<31;i++)
			{
				clickObjback.transform.Find("Signfinishlist").Find("signbg"+i).gameObject.SetActive(false);
				clickObjfront.transform.Find("Signlist").Find("signbg"+i).gameObject.SetActive(true);
			}
		}
		
	}
	IEnumerator POST()
	{

		WWW www = new WWW (URLPath);
		yield return www;
		if (www.error != null) {
			TimeRead = "error:" + www.error;
		} else {
			
			TimeRead=www.text;
		}
		RefreshData ();
		
	}
	void OnSignClick(GameObject go)
	{
		
		//Debug.Log("TimeRead="+TimeRead);
		if (qd1.activeSelf) {
			qd1.SetActive (false);
			qd2.SetActive (true);	
			redObj.SetActive(false);
			SignTime=TimeRead;
			SignTimeArray=SignTime.Split('-');
			//			SignTime=DayTime;
			//			SignTimeArray=SignTime.Split('/');
			++count;
			//count=++PlayerPrefs.GetInt("count");
			clickObjback.transform.Find("signbg"+count).GetComponent<UISprite>().enabled=true;
			clickObjfront.transform.Find("signbg"+count).GetComponent<UISprite>().enabled=false;
			clickObjback.transform.Find("signbg"+count).gameObject. GetComponent<Animation>().Play("signani");

			string[] daojuname={"乌鸦魔法","恶魔乌鸦魔法","权杖魔法","绿雾魔法","风之翼魔法","金风魔法","狼魔法","荆棘丛生魔法"};
			string[] daoju={"Crow_Prop_Count","Crow2nd_Prop_Count","TheStaffCost_Prop_Count","GreenMagicCost_Prop_Count","WingWindCost_Prop_Count","YellowPixieDustCost_Prop_Count","WolfHowlCost_Prop_Count","ThorwnCost_Prop_Count"};
			//	int djsel=UnityEngine.Random.Range(0,8);
			//	CostItemManager.mInstance.AddCount (1, daoju[djsel]);
			//	signtext.text="获取"+SumCount+"个魔法值和"+"1个"+daojuname[djsel];
			if(count==6||count==11||count==21||count==29)
			{
				RewardArray=Reward[count].Split(',');
				SumCount=Convert.ToInt32(RewardArray[0]);
				TokensSystem.Instance.AddMana (SumCount);
				int getdjnum=Convert.ToInt32(RewardArray[1]);
				int getdjid=Convert.ToInt32(RewardArray[2]);
				CostItemManager.mInstance.AddCount(getdjnum,daoju[getdjid]);
				string result="今日获取"+SumCount+"个魔法值和"+getdjnum+"个"+daojuname[getdjid];
				if(count<=7)
				{
					signtext0.text=result;
					signtext0.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=14){
					signtext1.text=result;
					signtext1.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=21){
					signtext2.text=result;
					signtext2.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=31){
					signtext3.text=result;
					signtext3.gameObject. GetComponent<Animation>().Play("signresult");
				}
				
				//print("getreward="+SumCount+"魔法和"+getdjnum+"个"+daojuname[getdjid]);
			}else{
				SumCount=Convert.ToInt32(Reward[count]);
				TokensSystem.Instance.AddMana (SumCount);
				string result="今日获取"+SumCount+"个魔法值";
				if(count<=7)
				{
					signtext0.text=result;
					signtext0.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=14){
					signtext1.text=result;
					signtext1.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=21){
					signtext2.text=result;
					signtext2.gameObject. GetComponent<Animation>().Play("signresult");
				}else if(count<=31){
					signtext3.text=result;
					signtext3.gameObject. GetComponent<Animation>().Play("signresult");
				}
			}
			issign=true;
			//	PlayerPrefs.SetString ("issign", issign.ToString ());
			
			PlayerPrefs.SetString ("SignTime", SignTime);
			PlayerPrefs.SetInt("count",count);
			PlayerPrefs.SetString("issign",issign.ToString());
			PlayerPrefs.Save();
			//isqded=false;
		}
		
	}
	
	
}
